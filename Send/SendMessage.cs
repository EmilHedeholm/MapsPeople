using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Newtonsoft.Json;
using DataModels;
using RabbitMQ.Client.Exceptions;

namespace Send {
    public class SendMessage {
        //This method receives mapped messasages from CoreForRabbitMQ and use RabbitMQ to send messages to the system  
        //Param: Is a list of external models. 
        public void SendUpdate(List<ExternalModel> messages) {
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

