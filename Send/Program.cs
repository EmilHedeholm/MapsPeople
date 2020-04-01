using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using RabbitMQ.Client;

namespace Send {
    class Program {
        static void Main(string[] args) {

            Stack<string> parentIds = new Stack<string>();
            for (int i = 1; i <= 4; i++) {
                string parent = "" + i;
                parentIds.Push(parent);
            }
            Source source = new Source();

            ExternalModel externalMessage = new ExternalModel() { Source = source, ParentIds = parentIds };
            List<ExternalModel> externals = new List<ExternalModel>();
            externals.Add(externalMessage);

            //string routingKey = "", bindingKey = "", queueId = "";
            //int repeatTimes = externalMessage.ParentIds.Count;
            //for (int i = 0; i < repeatTimes; i++) {
            //    if (externalMessage.ParentIds.Count > 1) {
            //        queueId = externalMessage.ParentIds.Pop();
            //        routingKey += queueId + ".";
            //        bindingKey = routingKey + "#";
            //    } else {
            //        queueId = externalMessage.ParentIds.Pop();
            //        routingKey += queueId;
            //        bindingKey = routingKey;
            //    }
            //    Console.WriteLine("QueueId: " + queueId + ", " + "bindingKey: " + bindingKey);
            //}
            //Console.WriteLine("routingKey: " + routingKey);

            SendMessage sender = new SendMessage();
            sender.SendUpdate(externals);

            ConnectionFactory connectionFactory = new ConnectionFactory {
                HostName = "localhost"
            };
            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            // accept only one unack-ed message at a time

            // uint prefetchSize, ushort prefetchCount, bool global

            channel.BasicQos(0, 1, false);

            MessageReceiver messageReceiver = new MessageReceiver(channel);

            channel.BasicConsume("test", false, messageReceiver);

            Console.ReadLine();
        }
    }
}