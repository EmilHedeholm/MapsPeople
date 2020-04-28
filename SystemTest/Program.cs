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
            Stopwatch sendWatch = new Stopwatch();
            while (true) {
                //Wait for 3 sek. 
                Thread.Sleep(3000);
                sendWatch.Start();
                List<Location> data = GetData();
                if (!(data.Count == 0)) {
                    SendDataWithRabbitMQ(data);
                }
                string userQueue = "testUser";
                string queueID = "287d4074d6c647a49f215fb1";
                Receiver receiver = new Receiver();
                receiver.Consume(userQueue, queueID);

                sendWatch.Stop();
                // Get the elapsed time as a TimeSpan value.
                TimeSpan ts = sendWatch.Elapsed;
                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine("Receive RunTime " + elapsedTime);
                sendWatch.Reset();
            }    
            
        }

      
        //This method gets data from the test data source provided by MapsPeople, and uses the method FilterData on that data. After that it returns a list of filtered data that has been converted to Internal Data Model by using the method ConvertFromJsonToInternalModel. 
        //Return: Is a list of locations in the internal Data model format. 
        private static List<Location> GetData() {
            //string jsonstr;
            //var request = WebRequest.Create("https://mi-ucn-live-data.azurewebsites.net/occupancy?datasetid=6fbb3035c7e2436ba335edac") as HttpWebRequest;
            //var response = request.GetResponse();
            //using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
            //    jsonstr = sr.ReadToEnd();
            //}
            ////The Data is in JSON, so it is deserialized so that it will be objects instead. 
            //List<RootObject> data = JsonConvert.DeserializeObject<List<RootObject>>(jsonstr);

            //List<RootObject> filteredData = FilterData(data);
            //return ConvertToInternalModel(filteredData);
            List<Location> data = new List<Location>();
            for (int i = 1; i < 201; i++) {
                Location location = new Location();
                location.Id = i + "";
                location.ConsumerId = 3;
                Source source = new Source();
                State state = new State();
                state.Property = "testState";
                Random rand = new Random();
                state.Value = rand.Next() % 2 == 0 ? true.ToString() : false.ToString();
                source.State.Add(state);
                source.Type = "testSource";
                source.TimeStamp = DateTime.Now;
                location.Sources.Add(source);
                data.Add(location);
            }
            return data;
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
