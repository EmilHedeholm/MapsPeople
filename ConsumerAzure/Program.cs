using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using ConsumerAzure.JsonModel;
using DataModels;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RestSharp;

namespace ConsumerAzure {
    //This class is a consumer to the test data source provided by MapsPeople. The test source represents data from a customers API.
    public class Program {

        static List<RootObject> oldData = new List<RootObject>();
        static string MessageBroker { get; set; }
        public static void Main(string[] args) {
            var choice = true;
            while (choice) {
                Console.WriteLine("input the name of the messagebroker you want to use(kafka, rabbitmq)");
                MessageBroker = Console.ReadLine();
                switch (MessageBroker) {
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
           while (true) {
                //Wait for 3 sek. 
                Thread.Sleep(3000);
                List<DataModels.Location> data = GetData();
                if (!(data.Count == 0)) {
                    if (MessageBroker.Equals("kafka")) {
                        SendUpdateWithKafka(data);
                    } else if (MessageBroker.Equals("rabbitmq")) {
                        SendUpdateWithRabbitMQ(data);
                    }
                }
            }
        }

        //This method gets data from the test data source provided by MapsPeople, and uses the method FilterData on that data. After that it returns a list of filtered data that has been converted to Internal Data Model by using the method ConvertFromJsonToInternalModel. 
        //Return: Is a list of locations in the internal Data model format. 
        private static List<Location> GetData() {
            string jsonstr;
            var request = WebRequest.Create("https://mi-ucn-live-data.azurewebsites.net/occupancy?datasetid=6fbb3035c7e2436ba335edac") as HttpWebRequest;
            var response = request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                jsonstr = sr.ReadToEnd();
            }
            //The Data is in JSON, so it is deserialized so that it will be objects instead. 
            List<RootObject> data = JsonConvert.DeserializeObject<List<RootObject>>(jsonstr);

            List<RootObject> filteredData = FilterData(data);
            return ConvertToInternalModel(filteredData);
        }

        //This method Converts data from the deserialized JSON to the Internal Datamodel format. 
        //Param: a list of RootObject objects. 
        //Return: A list of locations (internal)   
        private static List<Location> ConvertToInternalModel(List<RootObject> filteredData) {
            List<Location> locations = new List<Location>();
            //Goes through all Root objects - and makes a new Location object for each for them, and gives an ID.
            foreach (RootObject r in filteredData) {
                Location location = new Location();
                location.ExternalId = r.SpaceRefId;
                location.ConsumerId = 1;
                //Goes through the list LastReports in Root objects. - and makes a new Source object for each of them, and sets state, Type and TimeStamp. 
                foreach (LastReport lr in r.LastReports) {
                    Source source = new Source();
                    source.Type = "Occupancy";
                    source.TimeStamp = lr.TimeStamp;
                    State MotionDetected = new State() { Property = "MotionDetected", Value = lr.MotionDetected.ToString() };
                    State PersonCount = new State() { Property = "PersonCount", Value = lr.PersonCount.ToString() };
                    State SignsOfLife = new State() { Property = "SignsOfLife", Value = lr.SignsOfLife.ToString() };
                    //The new state is added to the state in a source. 
                    source.State.Add(MotionDetected);
                    source.State.Add(PersonCount);
                    source.State.Add(SignsOfLife);
                    //Adds source to the list of sources, in a location. 
                    location.Sources.Add(source);
                }
                //adds location to a list of locations. 
                locations.Add(location);
            }
            return (locations);
        }

        //This method filters data so that it only keeps data that has been changed. 
        //Param: a list of RootObject objects 
        private static List<RootObject> FilterData(List<RootObject> data) {
            List<RootObject> filteredData = new List<RootObject>();
            //If the list filteredData is empty then add all the data from the list rawData. 
            if (oldData.Count == 0) {
                oldData.AddRange(data);
                filteredData.AddRange(data);
            }
            //Goes through the raw data list. 
            foreach (RootObject r in data) {
                //Goes through a list that has data that has changed. 
                foreach (RootObject rO in oldData) {
                    //Checks that it is the same Root object. 
                    if (r.Id.Equals(rO.Id)) {
                        //Goes through the list of Last reports in the Root Object, but only those that are in raw data. 
                        foreach (LastReport l in r.LastReports) {
                            //Goes though the list of Last reports in the Root Object, but only those that are in filtered data.  
                            foreach (LastReport lr in rO.LastReports) {
                                //Checks that it is the same last report. 
                                if (l.Id.Equals(lr.Id)) {
                                    //If the property MotionDetected has changed. 
                                    if (!(l.MotionDetected.Equals(lr.MotionDetected))) {
                                        //And if it doesn't already exists in the list temp - Then add it to the list. 
                                        if (!filteredData.Contains(r)) {
                                            filteredData.Add(r);
                                        }
                                    }
                                    if (!(l.PersonCount.Equals(lr.PersonCount))) {
                                        if (!filteredData.Contains(r)) {
                                            filteredData.Add(r);
                                        }
                                    }
                                    if (!(l.SignsOfLife.Equals(lr.SignsOfLife))) {
                                        if (!filteredData.Contains(r)) {
                                            filteredData.Add(r);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //If there is something in the temp list, then the list filteredData will be cleared first, before the list temp is added. 
            List<RootObject> temp = new List<RootObject>();
            if (!(filteredData.Count == 0)) {
                oldData = data;
            }
            return filteredData;
        }

        //This method sends data to the Core Controller with Kafka. 
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
                    //Asynchronously send a message to the topic.
                    var deliveryReport = await producer.ProduceAsync(
                        topic, new Message<string, string> { Key = null, Value = json });

                    Console.WriteLine($"delivered to: {deliveryReport.TopicPartitionOffset}");
                } catch (ProduceException<string, string> e) {
                    Console.WriteLine($"failed to deliver message: {e.Message} [{e.Error.Code}]");
                }
            }
        }

        //This message sends data to the Core controller with RabbitMQ. 
        //Param: data is a list of locations. 
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
