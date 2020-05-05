using Confluent.Kafka;
using Confluent.Kafka.Admin;
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
    class Program {
        static string messageBroker { get; set; }
        public static void Main(string[] args) {
            var choice = true;
            while (choice) {
                Console.WriteLine("input the name of the messagebroker you want to use(kafka, rabbitmq)");
                messageBroker = Console.ReadLine();
                switch (messageBroker) {
                    case "kafka":
                        choice = false;
                        break;
                    case "rabbitmq":
                        choice = false;
                        break;
                    default:
                        Console.WriteLine("not a recognized messagebroker, try again");
                        break;
                }
            }
           // while (true) {
                //Wait for 3 sek. 
                Thread.Sleep(3000);
                List<DataModels.Location> data = GetData();
                if (!(data.Count == 0)) {
                    if (messageBroker.Equals("kafka")) {
                        SendUpdateWithKafka(data);
                    } else if (messageBroker.Equals("rabbitmq")) {
                        SendUpdateWithRabbitMQ(data);
                    }
                }
            //}
        }

        //This method request a token using a post Request using your credentials from Mapspeoples CMS, which gives you a token that you
        //pass along when interacting with Mapspeoples systems
        //Return Security Token
        public static Token GetToken(RestClient client)
        {
            client.BaseUrl = new Uri("https://auth.mapsindoors.com/connect/token");
            var request = new RestRequest(Method.POST);
            string encodedBody = string.Format("grant_type=password&client_id=client&username=1061951@ucn.dk&password=T40M51zt4MF0f9NV");
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
            string datasetId = datasets[0].Id;
            //This step to get geodata from Mapspeople
            var geodataRequest = new RestRequest($"/{datasetId}/api/geodata/", Method.GET);
            geodataRequest.AddHeader("authorization", response.token_type + " " + response.access_token);
            var geodataResponse = client.Execute(geodataRequest);
            jsonstr = geodataResponse.Content;                 
            List<RootObject> sources = JsonConvert.DeserializeObject<List<RootObject>>(jsonstr);
           
            return ConvertFromJsonToInternalModel(sources);
        }

        //This method Converts data from the deserialized JSON to the Internal Datamodel format. 
        //Param: a list of RootObject objects. 
        //Return: A list of locations (internal) 
        private static List<Location> ConvertFromJsonToInternalModel(List<RootObject> sources) {
            List<Location> locations = new List<Location>();
       
            foreach (RootObject r in sources) {
                Location location = new Location();
                location.ConsumerId = 2;
                location.Id = r.id;
                location.ExternalId = r.externalId;
                location.ParentId = r.parentId;
                locations.Add(location);
            }
            return (locations);
        }

        //This method sends data to the Core Controller for RabbitMQ. 
        //Param: Is a list of locations. 
        private static async void SendUpdateWithKafka(List<DataModels.Location> data) {
            var topic = "Consumer_Topic";
            using (var adminClient = new AdminClientBuilder(new AdminClientConfig { BootstrapServers = "localhost" }).Build()) {
                try {
                    await adminClient.CreateTopicsAsync(new TopicSpecification[] {
                                                                new TopicSpecification { Name = topic,
                                                                                         ReplicationFactor = 1,
                                                                                         NumPartitions = 1 }
                                                                });
                } catch (CreateTopicsException e) {
                }
            }
            using (var producer = new ProducerBuilder<string, string>(new ProducerConfig { BootstrapServers = "localhost" }).Build()) {
                try {
                    string json = JsonConvert.SerializeObject(data);
                    var deliveryReport = await producer.ProduceAsync(
                        topic, new Message<string, string> { Key = null, Value = json });

                    Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                } catch (ProduceException<string, string> e) {
                    Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                }
            }
        }

        private static void SendUpdateWithRabbitMQ(List<DataModels.Location> data) {
            try {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel()) {
                    channel.QueueDeclare(queue: "Consumer_Queue",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var message = JsonConvert.SerializeObject(data);
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: "Consumer_Queue",
                                         basicProperties: properties,
                                         body: body);
                    Console.WriteLine(message);
                    Console.WriteLine();
                    Console.WriteLine();
                }
            } catch (Exception e) {
                if (e is AlreadyClosedException) {
                    Console.WriteLine("The connectionis already closed");
                } else if (e is BrokerUnreachableException) {
                    Console.WriteLine("The broker cannot be reached");
                } else if (e is OperationInterruptedException) {
                    Console.WriteLine("The operation was interupted");
                } else if (e is ConnectFailureException) {
                    Console.WriteLine("Could not connect to the broker broker");
                } else {
                    Console.WriteLine("Something went wrong");
                }
            }
        }
    }
}
