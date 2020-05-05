﻿using System;
using System.Collections.Generic;
using System.Text;
using Confluent.Kafka;
using DataModels;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace ClientGUI {
    internal class Receiver {
        //This method takes a username and a queue ID and creates a queue with the username and binds it to the selected queue id
        //afterwards it creates the consumer and links it to the queue.
        public ExternalModel Consume(string userQueue, string queueID) {
             ExternalModel message1 = null;
            try {
                ConnectionFactory factory = new ConnectionFactory();
                factory.HostName = "localhost";
                IConnection conn = factory.CreateConnection();
                IModel channel = conn.CreateModel();
                channel.ExchangeDeclare(exchange: "Customer1",
                                            type: "topic");
                var args = new Dictionary<string, object>();
                args.Add("x-message-ttl", 30000);

                channel.QueueDeclare(queue: userQueue, durable: true,
                                   exclusive: false,
                                   autoDelete: true,
                                   arguments: args);
                channel.QueueBind(queue: userQueue, exchange: "Customer1", routingKey: $"#.{queueID}.#");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) => {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    if (message != null) {
                         message1 = JsonConvert.DeserializeObject<ExternalModel>(message);
                        //foreach (var parentId in deserializedMessage.ParentIds){
                        //    Console.Write(parentId + ", ");
                        //    Console.WriteLine();
                        //}
                        Console.WriteLine(message);
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
            return message1;
        }

        public ExternalModel ReceiveDataFromKafka(string userName, string topic) {
            ExternalModel message = null;
            using (var consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig { BootstrapServers = "localhost", GroupId = userName }).Build()) {
                consumer.Subscribe(topic);
                while (true) {
                    var result = consumer.Consume();
                    message = JsonConvert.DeserializeObject <ExternalModel> (result.Message.Value);
                    Console.WriteLine(result.Message.Value);
                    return message;
                }              
            }          
        }
    }
}