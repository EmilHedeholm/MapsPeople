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
            Console.WriteLine("Enter Username");
            string userName = Console.ReadLine();
            string database = "";
            while (database != "neo4j" && database != "mongodb" && database != "mssql") { 
            Console.WriteLine("Enter the name of the database you want to use(neo4j, mongodb, mssql)");
            database = Console.ReadLine();
            }
            while (choice) {
                Console.WriteLine("input the name of the messagebroker you want to use(kafka, rabbitmq)");
                var messageBroker = Console.ReadLine();
                switch (messageBroker) {
                    case "kafka":
                        Console.WriteLine("Enter a topic");
                        string topic = Console.ReadLine();
                        GetAllLocations(topic, database);
                        receiver.ReceiveDataFromKafka(userName, topic);
                        choice = false;
                        break;
                    case "rabbitmq":
                        Console.WriteLine("Enter a queue ID");
                        string queueID = Console.ReadLine();
                        GetAllLocations(queueID, database);
                        receiver.Consume(userName, queueID);
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
