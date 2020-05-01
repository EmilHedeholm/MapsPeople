using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;

namespace SystemTest {
    public class KafkaTest {
        public void ReceiveDataFromKafka(string userName, string topic) {
            using (var consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig { BootstrapServers = "localhost", GroupId = userName }).Build()) {
                consumer.Subscribe(topic);
                var result = consumer.Consume();
                Console.WriteLine("Message Received");
            }
        }
        public async void SendDataWithKafka(List<Location> locations) {
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
    }
}
