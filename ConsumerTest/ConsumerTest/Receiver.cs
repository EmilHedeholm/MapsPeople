using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ConsumerTest
{
    class Receiver
    {
        public void Consume()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.HostName = "localhost";
            IConnection conn = factory.CreateConnection();
            IModel channel = conn.CreateModel();
            while (true){

                Console.WriteLine("Enter a queue ID");
                string queueID = Console.ReadLine();
                var response = channel.QueueDeclarePassive(queueID);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) =>{
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    if (message != null) {
                        var deserializedMessage = JsonConvert.DeserializeObject<ExternalModel>(message);
                        foreach (var parentId in deserializedMessage.ParentIds){
                            Console.Write(parentId + ", ");
                            Console.WriteLine();
                        }
                    }
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: queueID,
                                     autoAck: false,
                                     consumer: consumer);
                Console.ReadLine();
            }
        }
    }
}




