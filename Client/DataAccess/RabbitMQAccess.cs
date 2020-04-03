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

        public Message Data { get; set; }
        public void ReceiveDataFromRabbitMQ(string queueName) {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {
                channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                //channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += Consumer_Received;
                channel.BasicConsume(queue: queueName,
                                     autoAck: false,
                                     consumer: consumer);
            }
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs) {
            var message = Encoding.UTF8.GetString(eventArgs.Body);
            if (message != null) {
                //The message is converted from JSON to IEnumerable<Location>.
                Data = JsonConvert.DeserializeObject<Message>(message);
            }
        }
    }
}
