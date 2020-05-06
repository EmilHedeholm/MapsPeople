using Confluent.Kafka;
using Confluent.Kafka.Admin;
using DatabaseAccess;
using DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MessageBrokers {
    //This class receives uses Kafka and receives the mapped data from Receive, and sends the data to the users. 
    public class MessageBrokerKafka : IMessageBroker {
        HashSet<string> CreatedKafkaTopics { get; set; }

        //This method receives data from a consumer, and then sends the data to a user.
        public void HandleUpdateFromConsumer(IDataAccess dataAccess) {
            Receiver.Receiver receiver = new Receiver.Receiver();
            //A list of the topics we already created on kafka. This improves performance.
            CreatedKafkaTopics = new HashSet<string>();
            var topic = "Consumer_Topic";
            using (var consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig { BootstrapServers = "localhost", GroupId = "Core" }).Build()) {
                consumer.Subscribe(topic);
                while (true) {
                    var consumedMessage = consumer.Consume();
                    var deserializedMessage = JsonConvert.DeserializeObject<List<Location>>(consumedMessage.Message.Value);
                    var result =receiver.Receive(deserializedMessage, dataAccess);
                    if(result != null) {
                        SendUpdateToUsers(result);
                    }
                }
            }
        }


        private async void SendUpdateToUsers(List<Message> messages) {
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost" }).Build()) {
                Metadata metadata = adminClient.GetMetadata(new TimeSpan(0, 0, 1));
                foreach (var topic in metadata.Topics) {
                    CreatedKafkaTopics.Add(topic.Topic);
                }
                foreach (var externalMesssage in messages) {
                    foreach (var parentId in externalMesssage.ParentIds) {
                        if (!CreatedKafkaTopics.Contains(parentId)) {
                            try {
                                await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                                                                new TopicSpecification { Name = parentId,
                                                                                         ReplicationFactor = 1,
                                                                                         NumPartitions = 1 }
                                                                });
                                CreatedKafkaTopics.Add(parentId);
                            } catch (CreateTopicsException e) {
                                Console.WriteLine($"An error occured while creating topic: {e.Error.Reason}");
                                if (e.Error.Reason.Contains("already exists")) {
                                    CreatedKafkaTopics.Add(parentId);
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
