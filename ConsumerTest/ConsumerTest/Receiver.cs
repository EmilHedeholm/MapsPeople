using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsumerTest {
    class Receiver {
        public ExternalModel Consume() {

            ConnectionFactory factory = new ConnectionFactory();
            // "guest"/"guest" by default, limited to localhost connections
            factory.HostName = "localhost";

            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();

            var response = channel.QueueDeclarePassive("287d4074d6c647a49f215fb1");
            
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, ea) => {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                if (message != null) {
                    //The message is converted from JSON to IEnumerable<Location>.
                    var deserializedMessage = JsonConvert.DeserializeObject<ExternalModel>(message);
                    foreach (var parentId in deserializedMessage.ParentIds) {
                        Console.Write(parentId + ", ");
                        Console.WriteLine();
                    }
                    //Console.WriteLine(message + "\n\n");

                }
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: "287d4074d6c647a49f215fb1",
                                 autoAck: false,
                                 consumer: consumer);
            Console.ReadLine();

            return null;
        }
    }
}
