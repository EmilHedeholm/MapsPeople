using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using Newtonsoft.Json;
using DataModels;
using KafkaNet;
using KafkaNet.Model;
using KafkaNet.Protocol;

namespace Send {
    public class SendMessage {

        public void SendUpdate(List<ExternalModel> messages) {

            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {
                channel.ExchangeDeclare(exchange: "Customer1",
                                        type: "topic");
                //Setting Time to live of the message
                var args = new Dictionary<string, object>();
                args.Add("x-message-ttl", 30000);
                
                foreach (var externalMessage in messages) {
                    string routingKey = "", bindingKey = "", queueId = "";
                    int repeatTimes = externalMessage.ParentIds.Count;
                    Stack<string> parentsForDelivery = new Stack<string>(externalMessage.ParentIds);

                    //Iterating through the parent id's to create queues, routingkeys and bindings.
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
                               arguments: args);
                        channel.QueueBind(queue: queueId, exchange: "Customer1", routingKey: $"{bindingKey}");

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
        }

        //private void DistributeMessage(string topicName, List<ExternalModel> messages) {
        //    foreach (var message in messages) {
        //        var jsonString = JsonConvert.SerializeObject(message);
        //        var topic = topicName;
        //        Message msg = new Message(jsonString);
        //        Uri uri = new Uri("http://localhost:9092");
        //        var options = new KafkaOptions(uri);
        //        var router = new BrokerRouter(options);
        //        var client = new Producer(router);

        //        client.SendMessageAsync(topic, new List<Message> { msg }).Wait();
        //    }
            
        //}

        //public void SendUpdateWithKafka(List<ExternalModel> messages) {
        //    foreach (var externalMessage in messages) {
        //        Stack<string> parentsForDelivery = new Stack<string>(externalMessage.ParentIds);
        //        int repeatTimes = externalMessage.ParentIds.Count;
        //        string topic = "";
        //        for (int i = 0; i < repeatTimes; i++) {
        //            topic= externalMessage.ParentIds.Pop();
        //            DistributeMessage(topic, messages);
        //        }


        //    }
        //}
    }

}
