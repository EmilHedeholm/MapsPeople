using Confluent.Kafka;
using Confluent.Kafka.Admin;
using DatabaseAccess;
using DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Receiver;

namespace MessageBrokers {
    public class MessageBrokerKafka : IMessageBroker {
        HashSet<string> createdKafkaTopics { get; set; }
        public void ReceiveUpdateFromConsumer(IDataAccess dataAccess) {
            Reciever reciever = new Reciever();
            //A list of the topics we already created on kafka. This improves performance.
            createdKafkaTopics = new HashSet<string>();
            var topic = "Consumer_Topic";
            using (var consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig { BootstrapServers = "localhost", GroupId = "Core" }).Build()) {
                consumer.Subscribe(topic);
                while (true) {
                    var consumedMessage = consumer.Consume();
                    var deserializedMessage = JsonConvert.DeserializeObject<List<Location>>(consumedMessage.Message.Value);
                    var result =reciever.Receive(deserializedMessage, dataAccess);
                    if(result != null) {
                        SendUpdateToUsers(result);
                    }
                }
            }
        }

        public async void SendUpdateToUsers(List<Message> messages) {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost" }).Build()) {
                Metadata metadata = adminClient.GetMetadata(new TimeSpan(0, 0, 1));
                foreach (var topic in metadata.Topics) {
                    createdKafkaTopics.Add(topic.Topic);
                }
                foreach (var externalMesssage in messages) {
                    foreach (var parentId in externalMesssage.ParentIds) {
                        if (!createdKafkaTopics.Contains(parentId)) {
                            try {
                                await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                                                                new TopicSpecification { Name = parentId,
                                                                                         ReplicationFactor = 1,
                                                                                         NumPartitions = 1 }
                                                                });
                                createdKafkaTopics.Add(parentId);
                            } catch (CreateTopicsException e) {
                                Console.WriteLine($"An error occured while creating topic: {e.Error.Reason}");
                                if (e.Error.Reason.Contains("already exists")) {
                                    createdKafkaTopics.Add(parentId);
                                }

                            }
                        }

                        using (var producer = new ProducerBuilder<string, string>(new ProducerConfig { BootstrapServers = "localhost" }).Build()) {
                            try {
                                string json = JsonConvert.SerializeObject(externalMesssage);
                                var deliveryReport = await producer.ProduceAsync(
                                    parentId, new Message<string, string> { Key = null, Value = json });

                                Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                            } catch (ProduceException<string, string> e) {
                                Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                            }
                        }
                    }
                }
            }
        }
    }
}
