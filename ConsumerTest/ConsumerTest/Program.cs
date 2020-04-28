using DataModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerTest {
    class Program {
        static void Main(string[] args) {
            Receiver receiver = new Receiver();
            var choice = true;
            while (choice) {
                Console.WriteLine("input the name of the messagebroker you want to use(kafka, rabbitmq)");
                var messageBroker = Console.ReadLine();
                switch (messageBroker) {
                    case "kafka":
                        Console.WriteLine("Enter a topic");
                        string topic = Console.ReadLine();
                        Console.WriteLine("Enter the name of the database you want to use(neo4j, mongodb, mssql)");
                        string databasek = Console.ReadLine();
                        GetAllLocations(topic, databasek);
                        receiver.ReceiveDataFromKafka(topic);
                        choice = false;
                        break;
                    case "rabbitmq":
                        Console.WriteLine("Enter Username");
                        string userQueue = Console.ReadLine();
                        Console.WriteLine("Enter a queue ID");
                        string queueID = Console.ReadLine();
                        Console.WriteLine("Enter the name of the database you want to use(neo4j, mongodb, mssql)");
                        string databaser = Console.ReadLine();
                        GetAllLocations(queueID, databaser);
                        receiver.Consume(userQueue, queueID);
                        choice = false;
                        break;
                    default:
                        Console.WriteLine("not a recognized messagebroker, try again");
                        break;
                }
            }  
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
