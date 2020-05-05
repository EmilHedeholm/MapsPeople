using Confluent.Kafka;
using System;
using System.Collections.Generic;
using DataModels;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;

namespace SystemTest {
    public class KafkaTest {

        IConsumer<Ignore, string> consumer { get; set; }

        public KafkaTest(string userName, string topic) {
            consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig { BootstrapServers = "localhost", GroupId = userName }).Build();
            consumer.Subscribe(topic);
        }
        public void ReceiveDataFromKafka(string userName, string topic) {
           // using (consumer) {
                var result = consumer.Consume();
                Console.WriteLine("Message Received");
            //}
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
