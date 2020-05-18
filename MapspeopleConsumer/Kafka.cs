using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerAzure {
    public class Kafka {

        IProducer<string, string> Producer { get; set; }
        public Kafka() {
            Producer = new ProducerBuilder<string, string>(new ProducerConfig { BootstrapServers = "localhost" }).Build();
        }

        //This method sends data to the Core Controller with Kafka. 
        //Param: Is a list of locations. 
        public async void SendUpdateWithKafka(List<DataModels.Location> data) {
            var topic = "Consumer_Topic";
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost" }).Build()) {
                try {
                    await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                                                        new TopicSpecification { Name = topic,
                                                                                 ReplicationFactor = 1,
                                                                                 NumPartitions = 2 }
                                                        });
                } catch (CreateTopicsException e) {
                }
            }

            try {
                string json = JsonConvert.SerializeObject(data);
                //Asynchronously send a message to the topic.
                var deliveryReport = await Producer.ProduceAsync(
                    topic, new Message<string, string> { Key = null, Value = json });

                Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
            } catch (ProduceException<string, string> e) {
                Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
            }
        }
    }
}
