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
            Console.WriteLine("Enter Username");
            string userQueue = Console.ReadLine();
            Console.WriteLine("Enter a queue ID");
            string queueID = Console.ReadLine();
            Console.WriteLine("Enter the name of the database you want to use(neo4j, mongodb, mssql)");
            string database = Console.ReadLine();
            GetAllLocations(queueID, database);
            Receiver receiver = new Receiver();
            receiver.Consume(userQueue, queueID);
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
