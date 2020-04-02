using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Client.Models;

namespace Client.DataAccess {
    public class RabbitMQAccess {

        public Message data { get; set; }
        public void ReceiveDataFromRabbitMQ(string queueName) {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) => {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    if (message != null) {
                        //The message is converted from JSON to IEnumerable<Location>.
                        data = JsonConvert.DeserializeObject<Message>(message);
                        
                    }
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);
            }
        }
    }
}