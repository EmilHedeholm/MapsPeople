using DatabaseAccess;
using DataModels;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using Receiver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MessageBrokers
{
    public class MessageBrokerRabbitMQ : IMessageBroker {
        public void ReceiveUpdateFromConsumer(IDataAccess dataAccess) {
            Reciever reciever = new Reciever();
            try {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel()) {
                    channel.QueueDeclare(queue: "Consumer_Queue",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    Console.WriteLine(" [*] Waiting for messages.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (sender, ea) => {
                        var body = ea.Body;
                        var message = Encoding.UTF8.GetString(body);
                        if (message != null) {
                            //The message is converted from JSON to IEnumerable<Location>.
                            var deserializedMessage = JsonConvert.DeserializeObject<IEnumerable<Location>>(message);
                            var result = reciever.Receive(deserializedMessage, dataAccess);
                            if(result != null) {
                                SendUpdateToUsers(result);
                            }
                        }
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "Consumer_Queue",
                                         autoAck: false,
                                         consumer: consumer);
                    Console.ReadLine();
                }
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

        public void RecieveUpdateFromCore(string userName, string LocationID) {
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

                channel.QueueDeclare(queue: userName, durable: true,
                                   exclusive: false,
                                   autoDelete: false,
                                   arguments: args);
                channel.QueueBind(queue: userName, exchange: "Customer1", routingKey: $"#.{LocationID}.#");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) => {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    if (message != null) {
                        //var deserializedMessage = JsonConvert.DeserializeObject<ExternalModel>(message);
                        //foreach (var parentId in deserializedMessage.ParentIds){
                        //    Console.Write(parentId + ", ");
                        //    Console.WriteLine();
                        //}
                        Console.WriteLine(message);
                    }
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: userName,
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

        public void SendUpdateToCore(List<Location> locations) {
            try {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel()) {
                    channel.QueueDeclare(queue: "Consumer_Queue",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var message = JsonConvert.SerializeObject(locations);
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: "Consumer_Queue",
                                         basicProperties: properties,
                                         body: body);
                    Console.WriteLine(message);
                    Console.WriteLine();
                    Console.WriteLine();
                }
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

        public void SendUpdateToUsers(List<ExternalModel> messages) {
            try {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel()) {
                    channel.ExchangeDeclare(exchange: "Customer1",
                                            type: "topic");

                    foreach (var externalMessage in messages) {
                        string routingKey = "", id = "";
                        int repeatTimes = externalMessage.ParentIds.Count;
                        Stack<string> parentsForDelivery = new Stack<string>(externalMessage.ParentIds);

                        //Iterating through the parent id's to create queues, routingkeys and bindings.
                        for (int i = 0; i < repeatTimes; i++) {
                            if (externalMessage.ParentIds.Count > 1) {
                                id = externalMessage.ParentIds.Pop();
                                routingKey += id + ".";
                            } else {
                                id = externalMessage.ParentIds.Pop();
                                routingKey += id;
                            }
                        }
                        externalMessage.ParentIds = parentsForDelivery;
                        string json = JsonConvert.SerializeObject(externalMessage);
                        var body = Encoding.UTF8.GetBytes(json);

                        channel.BasicPublish(exchange: "Customer1",
                                                routingKey: routingKey,
                                                basicProperties: null,
                                                body: body);
                        Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, json + "\n");
                    }
                }
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
    }
}
