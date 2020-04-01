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
            
            SendMessage.SendUpdate(hanser);


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
    public class ExternalHans {
        public List<string> ParentIds { get; set; }
        public Source source { get; set; }
    }
}