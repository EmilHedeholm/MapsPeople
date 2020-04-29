using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using ConsumerAzure.JsonModel;
using DataModels;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using RestSharp;
using System.Diagnostics;

namespace SystemTest {
    class Program {
        static List<RootObject> oldData = new List<RootObject>();
        
        static void Main(string[] args) {
            //Stopwatch stopWatch = new Stopwatch();
            //stopWatch.Start();
            //Thread.Sleep(10000);
            List<double> times = new List<double>();
            //Stopwatch sendWatch = new Stopwatch();
            string userQueue = "testUser";
            string queueID = "7f29498392eb4d40a1e17731";
            string spaceRefId = "GS202";
            for (int i = 0; i < 51; i++) {
                //Wait for 3 sek. 
                Thread.Sleep(3000);
                //sendWatch.Start();
                List<Location> data = GetData(spaceRefId);
                var start = DateTime.Now;
                if (!(data.Count == 0)) {
                    SendDataWithRabbitMQ(data);
                }
                Receiver receiver = new Receiver();
                receiver.Consume(userQueue, queueID);
                var stop = DateTime.Now;

                var elapsedTime = stop - start;
                //sendWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                //TimeSpan ts = sendWatch.Elapsed;
                //// Format and display the TimeSpan value.
                //string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                //    ts.Hours, ts.Minutes, ts.Seconds,
                //    ts.Milliseconds / 10);
                Console.WriteLine("Receive RunTime " + elapsedTime);
                times.Add(elapsedTime.TotalSeconds);
                //sendWatch.Reset();
            }
            double total = 0;
            foreach (var time in times) {
                total += time;
            }
            double average = total / 50;
            Console.WriteLine(average);
            Console.ReadLine();
        }

      
        //This method gets data from the test data source provided by MapsPeople, and uses the method FilterData on that data. After that it returns a list of filtered data that has been converted to Internal Data Model by using the method ConvertFromJsonToInternalModel. 
        //Return: Is a list of locations in the internal Data model format. 
        private static List<Location> GetData(string spaceRefId) {
            string jsonstr;
            var request = WebRequest.Create("https://mi-ucn-live-data.azurewebsites.net/occupancy?datasetid=6fbb3035c7e2436ba335edac") as HttpWebRequest;
            var response = request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                jsonstr = sr.ReadToEnd();
            }
            //The Data is in JSON, so it is deserialized so that it will be objects instead. 
            List<RootObject> data = JsonConvert.DeserializeObject<List<RootObject>>(jsonstr);
            List<RootObject> filteredData = FilterData(data, spaceRefId);
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
                //Goes through the list LastReports in Root objects. - and makes a new Source object for each of them, and sets state, ID, Type and TimeStamp. 
                foreach (LastReport lr in r.LastReports) {
                    Source source = new Source();
                    source.Type = "Occupancy";
                    source.TimeStamp = lr.TimeStamp;
                    State MotionDetected = new State() { Property = "MotionDetected", Value = lr.MotionDetected.ToString() };
                    State PersonCount = new State() { Property = "PersonCount", Value = lr.PersonCount.ToString() };
                    State SignsOfLife = new State() { Property = "SignsOfLife", Value = lr.SignsOfLife.ToString() };
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
        private static List<RootObject> FilterData(List<RootObject> data, string spaceRefId) {
            List<RootObject> filteredData = new List<RootObject>();
            foreach (var rootObject in data) {
                if (rootObject.SpaceRefId.Equals(spaceRefId)) {
                    filteredData.Add(rootObject);
                }
            }
            return filteredData;
        }

        //This method sends data to the Core Controller. 
        //Param: Is a list of locations. 
        private static void SendData(List<Location> locations) {
            var client = new RestClient();
            //TODO: indtastes post adresse
            client.BaseUrl = new Uri("https://localhost:44346/api/Receiving");
            string json = JsonConvert.SerializeObject(locations);
            var request = new RestRequest(Method.POST);
            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            var response = client.Execute(request);
            Console.WriteLine(json + "\n");
        }

        //This method sends data to the Core Controller for RabbitMQ. 
        //Param: Is a list of locations. 
        private static void SendDataWithRabbitMQ(List<Location> locations) {
            try {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel()) {
                    channel.QueueDeclare(queue: "Consumer_Queue",
                                         durable: true,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var message = JsonConvert.SerializeObject(locations);
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                                         routingKey: "Consumer_Queue",
                                         basicProperties: properties,
                                         body: body);
                    //Console.WriteLine(message);
                    //Console.WriteLine();
                    //Console.WriteLine();
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
