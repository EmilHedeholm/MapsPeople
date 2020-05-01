using DataModels;
using MessageBrokers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerTest {
    class Program {
        
        static IMessageBroker messageBroker { get; set; }
        static void Main(string[] args) {
            var choice = true;
            Console.WriteLine("Enter Username");
            string userName = Console.ReadLine();
            string database = "";
            while (database != "neo4j" && database != "mongodb" && database != "mssql") { 
            Console.WriteLine("Enter the name of the database you want to use(neo4j, mongodb, mssql)");
            database = Console.ReadLine();
            }
            Console.WriteLine("Enter a LocationID");
            string locationID = Console.ReadLine();
            while (choice) {
                Console.WriteLine("input the name of the messagebroker you want to use(kafka, rabbitmq)");
                var messageBrokerChoice = Console.ReadLine();
                switch (messageBrokerChoice) {
                    case "kafka":
                        messageBroker = new MessageBrokerKafka();
                        choice = false;
                        break;
                    case "rabbitmq":
                        messageBroker = new MessageBrokerRabbitMQ();
                        choice = false;
                        break;
                    default:
                        Console.WriteLine("not a recognized messagebroker, try again");
                        break;
                }
            }
            GetAllLocations(locationID, database);
            messageBroker.RecieveUpdateFromCore(userName, locationID);
        }

        private static void GetAllLocations(string queueId, string database) {
            string jsonstr;
            var request = WebRequest.Create($"https://localhost:44346/api/Send?id={queueId}&database={database}") as HttpWebRequest;
            var response = request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                jsonstr = sr.ReadToEnd();
            }
            Console.WriteLine(jsonstr);
        }
    }
}
