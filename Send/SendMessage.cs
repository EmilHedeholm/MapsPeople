using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Newtonsoft.Json;
using DataModels;

namespace Send {
    public class SendMessage {

        public static void SendUpdate(List<ExternalModel> messages) { 
            
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {
                channel.ExchangeDeclare(exchange: "Customer1",
                                        type: "topic");
                
                foreach (var externalMessage in messages) {
                    string routingKey = "";
                    foreach (var parentId in externalMessage.ParentIds) {
                        if (!externalMessage.ParentIds.Last().Equals(parentId)) {
                            routingKey += parentId + ".";
                        } else {
                            routingKey += parentId;
                        }
                    }
                    channel.QueueDeclare("test", durable: true,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
                    channel.QueueBind(queue: "test", exchange: "Venue1", routingKey:routingKey);
                    string json = JsonConvert.SerializeObject(externalMessage);
                    var body = Encoding.UTF8.GetBytes(json);
                    
                    channel.BasicPublish(exchange: "Venue1",
                                            routingKey: routingKey,
                                            basicProperties: null,
                                            body: body);
                    Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, json + "\n");
                }
                
            }
        }
    }


}
