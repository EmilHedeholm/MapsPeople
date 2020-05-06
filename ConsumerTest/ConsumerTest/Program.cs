using System;
using System.IO;
using System.Net;

namespace ConsumerTest {
    //This class uses the console to make a client that can get locations. 
    public class Program {
        static string MessageBroker { get; set; }
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
            Console.WriteLine("Enter a LocationID");
            string locationID = Console.ReadLine();
            GetAllLocations(locationID, database);
            while (choice) {
                Console.WriteLine();
                Console.WriteLine("input the name of the messagebroker you want to use(kafka, rabbitmq)");
                var messageBrokerChoice = Console.ReadLine();
                switch (messageBrokerChoice) {
                    case "kafka":
                        receiver.ReceiveUpdateFromKafka(userName, locationID);
                        choice = false;
                        break;
                    case "rabbitmq":
                        receiver.ReceiveUpdateFromRabbitMQ(userName, locationID);
                        choice = false;
                        break;
                    default:
                        Console.WriteLine("not a recognized messagebroker, try again");
                        break;
                }
            }     
        }

        //This method gets all locationId's from the API. 
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
