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
            List<State> states = new List<State>() { new State() { Id = "Raynolds", Property = "Uppercut", Value = "Over 9000" } };
            Source sovs = new Source() { Id = "1234567", State = states, TimeStamp = DateTime.Now, Type = "Scouter" };
            List<string> parentIds = new List<string>{ "Benny", "Jhonny", "Birger" };
            List<string> parentIdsIs = new List<string> { "Benny", "Jhonny", "Palle"};
            ExternalHans hans = new ExternalHans() { ParentIds = parentIds, source = sovs };
            ExternalHans hansi = new ExternalHans() { ParentIds = parentIdsIs, source = sovs };
            List<ExternalHans> hanser = new List<ExternalHans>();
            hanser.Add(hans);
            hanser.Add(hansi);
            string[] routing = {"Benny.Jhonny.Birger"};
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

            channel.BasicConsume("Benny.Jhonny.Birger", false, messageReceiver);

            Console.ReadLine();
        }
    }
    public class ExternalHans {
        public List<string> ParentIds { get; set; }
        public Source source { get; set; }
    }
}