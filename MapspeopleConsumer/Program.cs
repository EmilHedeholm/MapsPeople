using Confluent.Kafka;
using Confluent.Kafka.Admin;
using ConsumerAzure;
using DataModels;
using MapspeopleConsumer.JsonModel;
using MapspeopleConsumer.TokenModel;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MapspeopleConsumer {
    //This class is a consumer to the data from the MapsPeople CMS. 
    class Program {
        static string MessageBroker { get; set; }
        public static void Main(string[] args) {
            var choice = true;
            Kafka kafka = null;
            RabbitMQ rabbitMQ = null;
            while (choice) {
                Console.WriteLine("input the name of the messagebroker you want to use(kafka, rabbitmq)");
                MessageBroker = Console.ReadLine();
                switch (MessageBroker) {
                    case "kafka":
                        choice = false;
                        kafka = new Kafka();
                        break;
                    case "rabbitmq":
                        choice = false;
                        rabbitMQ = new RabbitMQ();
                        break;
                    default:
                        Console.WriteLine("not a recognized messagebroker, try again");
                        break;
                }
            }
                List<DataModels.Location> data = GetData();
                if (!(data.Count == 0)) {
                    if (MessageBroker.Equals("kafka")) {
                        kafka.SendUpdateWithKafka(data);
                    } else if (MessageBroker.Equals("rabbitmq")) {
                        rabbitMQ.SendUpdateWithRabbitMQ(data);
                    }
                }
                Console.ReadLine();
        }

        //This method request a token using a post Request using your credentials from Mapspeoples CMS, which gives you a token that you
        //pass along when interacting with Mapspeoples systems
        //Return Security Token
        public static Token GetToken(RestClient client)
        {
            Console.WriteLine("Enter Username");
            string username = Console.ReadLine();
            Console.WriteLine("Enter Password");
            string password = Console.ReadLine();
            client.BaseUrl = new Uri("https://auth.mapsindoors.com/connect/token");
            var request = new RestRequest(Method.POST);
            string encodedBody = string.Format($"grant_type=password&client_id=client&username={username}&password={password}");
            request.AddParameter("application/x-www-form-urlencoded", encodedBody, ParameterType.RequestBody);
            request.AddParameter("Content-Type", "application/x-www-form-urlencoded", ParameterType.HttpHeader);
            var response = client.Execute<Token>(request);

            return response.Data;
        }

        //This method gets data from the geodata provided by MapsPeople. After that it returns a list of data that has been converted to Internal Data Model by using the method ConvertFromJsonToInternalModel. 
        //Return: Is a list of locations in the internal Data model format. 
        private static List<Location> GetData() {
            string jsonstr;  
            //This step to get datasetId from Mapspeople
            var client = new RestClient();
            var response = GetToken(client);
            client.BaseUrl = new Uri("https://integration.mapsindoors.com");
            var testRequest = new RestRequest("/api/dataset/", Method.GET);
            testRequest.AddHeader("authorization", response.token_type + " " + response.access_token);
            var something = client.Execute(testRequest);
            string datasetJsonstr = something.Content;          
            List<Dataset> datasets= JsonConvert.DeserializeObject<List<Dataset>>(datasetJsonstr);
            List<RootObject> sources = null;
            if (datasets != null) {
                string datasetId = datasets[0].Id;
                //This step to get geodata from Mapspeople
                var geodataRequest = new RestRequest($"/{datasetId}/api/geodata/", Method.GET);
                geodataRequest.AddHeader("authorization", response.token_type + " " + response.access_token);
                var geodataResponse = client.Execute(geodataRequest);
                jsonstr = geodataResponse.Content;
                sources = JsonConvert.DeserializeObject<List<RootObject>>(jsonstr);
            } else {
                Console.WriteLine("Username or Password was wrong");
            }
            return ConvertFromJsonToInternalModel(sources);
        }

        //This method Converts data from the deserialized JSON to the Internal Datamodel format. 
        //Param: a list of RootObject objects. 
        //Return: A list of locations (internal) 
        private static List<Location> ConvertFromJsonToInternalModel(List<RootObject> sources) {
            List<Location> locations = new List<Location>();
            if (sources != null) {
                foreach (RootObject r in sources) {
                    Location location = new Location();
                    location.ConsumerId = 2;
                    location.Id = r.Id;
                    location.ExternalId = r.ExternalId;
                    location.ParentId = r.ParentId;
                    locations.Add(location);
                }
            }
            return (locations);
        }
    }
}
