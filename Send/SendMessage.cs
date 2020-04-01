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
                    string routingKey = "", bindingKey = "", queueId = "";
                    int repeatTimes = externalMessage.ParentIds.Count;
                    //foreach (var parentId in externalMessage.ParentIds) {
                    for (int i = 0; i < repeatTimes; i++) {
                        if (externalMessage.ParentIds.Count > 1) {
                            queueId = externalMessage.ParentIds.Pop();
                            routingKey += queueId + ".";
                            bindingKey = routingKey + "#";
                        } else {
                            queueId = externalMessage.ParentIds.Pop();
                            routingKey += queueId;
                            bindingKey = routingKey;
                        }
                        channel.QueueDeclare(queueId, durable: true,
                               exclusive: false,
                               autoDelete: false,
                               arguments: null);
                        channel.QueueBind(queue: queueId, exchange: "Customer1", routingKey: $"{bindingKey}");

                        //if(externalMessage.ParentIds.Count == 1) {
                        //    channel.QueueBind(queue: parentId, exchange: "Customer1", routingKey: $"{parentId}.#");
                        //}
                        //if (externalMessage.ParentIds.Count == 2) {
                        //    channel.QueueBind(queue: parentId, exchange: "Customer1", routingKey: $"*.{parentId}.#");
                        //}
                        //if (externalMessage.ParentIds.Count == 3) {
                        //    channel.QueueBind(queue: parentId, exchange: "Customer1", routingKey: $"#.{parentId}.#");
                        //}
                        //if (externalMessage.ParentIds.Count == 4) {
                        //    channel.QueueBind(queue: parentId, exchange: "Customer1", routingKey: $"#.{parentId}");
                        //}

                        //if (externalMessage.ParentIds.Peek() != null) {
                        //    routingKey += parentId + ".";
                        //} else {
                        //    routingKey += parentId;
                        //}
                    }
                    string json = JsonConvert.SerializeObject(externalMessage);
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "Customer1",
                                            routingKey: routingKey,
                                            basicProperties: null,
                                            body: body);
                    Console.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, json + "\n");
                }

            }
        }
    }


}
