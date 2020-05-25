using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Client.Models;
using RabbitMQ.Client.Exceptions;

namespace Client.DataAccess {
    public class RabbitMQAccess {

        public Message Data { get; set; }
        public void ReceiveDataFromRabbitMQ(string userQueue, string queueID) {
            try {
                ConnectionFactory factory = new ConnectionFactory();
                factory.HostName = "localhost";
                IConnection conn = factory.CreateConnection();
                IModel channel = conn.CreateModel();
                channel.ExchangeDeclare(exchange: "Customer1",
                                            type: "topic");
                var args = new Dictionary<string, object>();
                args.Add("x-message-ttl", 60000);
                args.Add("x-expires", 60000);

                channel.QueueDeclare(queue: userQueue, durable: true,
                                   exclusive: false,
                                   autoDelete: false,
                                   arguments: args);
                channel.QueueBind(queue: userQueue, exchange: "Customer1", routingKey: $"#.{queueID}.#");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) => {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    if (message != null) {
                       Data = JsonConvert.DeserializeObject<Message>(message);
                    }
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: userQueue,
                                     autoAck: false,
                                     consumer: consumer);
                Console.ReadLine();

            } catch (Exception e) {
                if (e is AlreadyClosedException) {
                    Console.WriteLine("The connectionis already closed");
                } else if (e is BrokerUnreachableException) {
                    Console.WriteLine("The broker cannot be reached");
                } else if (e is OperationInterruptedException) {
                    Console.WriteLine("The operation was interupted");
                } else if (e is ConnectFailureException) {
                    Console.WriteLine("Could not connect to the broker broker");
                } else {
                    Console.WriteLine("Something went wrong");
                }
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
