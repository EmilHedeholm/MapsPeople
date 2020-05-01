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
        public void ReceiveUpdateFromConsumer(IDataAccess dataAccess) {
            Reciever reciever = new Reciever();
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

        public void RecieveUpdateFromCore(string userName, string LocationID) {
            using (var consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig { BootstrapServers = "localhost", GroupId = userName }).Build()) {
                consumer.Subscribe(LocationID);
                while (true) {
                    var result = consumer.Consume();
                    Console.WriteLine(result.Message.Value);
                }
            }
        }

        public async void SendUpdateToCore(List<Location> locations) {
            var topic = "Consumer_Topic";
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost" }).Build()) {
                try {
                    await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                                                                new TopicSpecification { Name = topic,
                                                                                         ReplicationFactor = 1,
                                                                                         NumPartitions = 1 }
                                                                });
                } catch (CreateTopicsException e) {
                }
            }
            using (var producer = new ProducerBuilder<string, string>(new ProducerConfig { BootstrapServers = "localhost" }).Build()) {
                try {
                    string json = JsonConvert.SerializeObject(locations);
                    var deliveryReport = await producer.ProduceAsync(
                        topic, new Message<string, string> { Key = null, Value = json });

                    Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                } catch (ProduceException<string, string> e) {
                    Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                }
            }
        }

        public async void SendUpdateToUsers(List<ExternalModel> messages) {
            foreach (var externalMesssage in messages) {
                foreach (var parentId in externalMesssage.ParentIds) {
                    using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost" }).Build()) {
                        try {
                            await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                                                                new TopicSpecification { Name = parentId,
                                                                                         ReplicationFactor = 1,
                                                                                         NumPartitions = 1 }
                                                                });
                        } catch (CreateTopicsException e) {

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
