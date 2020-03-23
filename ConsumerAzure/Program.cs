using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsumerAzure.JsonModel;
using DataModels;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RestSharp;

namespace ConsumerAzure {
    class Program {

        static List<RootObject> filteredData = new List<RootObject>();

        //TODO: make this better polling and not garbage as it is right now
        static void Main() {
           while (true) {
                //Wait for 3 sek. 
                Thread.Sleep(3000);
                List<Location> data = GetData();
                if (!(data.Count == 0)) {
                    SendData(data);
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
            List<RootObject> sources = JsonConvert.DeserializeObject<List<RootObject>>(jsonstr);

            FilterData(sources);
            return ConvertToInternalModel(filteredData);
        }

        //This method Converts data from the deserialized JSON to the Internal Datamodel format. 
        //Param: a list of RootObject objects. 
        //Return: A list of locations (internal)   
        private static List<Location> ConvertToInternalModel(List<RootObject> sources) {
            List<Location> locations = new List<Location>();
            //Goes through all Root objects - and makes a new Location object for each for them, and gives an ID.
            foreach (RootObject r in sources) {
                Location location = new Location();
                location.ExternalId = r.SpaceRefId;
                location.ConsumerId = 1;
                //Goes through the list LastReports in Root objects. - and makes a new Source object for each of them, and sets state, ID, Type and TimeStamp. 
                foreach (LastReport lr in r.LastReports) {
                    Source source = new Source();
                    string MotionDetected = lr.MotionDetected.ToString();
                    string PersonCount = lr.PersonCount.ToString();
                    string SignsOfLife = lr.SignsOfLife.ToString();
                    source.State.Add("MotionDetected", MotionDetected);
                    source.State.Add("PersonCount", PersonCount);
                    source.State.Add("SignsOfLife", SignsOfLife);
                    source.Id = lr.Id;
                    source.Type = "Occupancy";
                    source.TimeStamp = lr.TimeStamp;
                    //Adds source to the list of sources, in a location. 
                    location.Sources.Add(source);
                }
                //adds location to a list of locations. 
                locations.Add(location);
            }
            return (locations);
        }

        //This method filters data so that it only keeps data that has been changed. 
        //Param: Json String. 
        private static void FilterData(List<RootObject> rawData) {
            //If the list filteredData is empty then add all the data from the list rawData. 
            if(filteredData.Count == 0) {
                filteredData.AddRange(rawData);
            }
            //A new list that can hold the changed data, before it is added to the list filteredData. 
            List<RootObject> temp = new List<RootObject>();
            //Goes through the raw data list. 
            foreach (RootObject r in rawData) {
                //Goes through a list that has data that has changed. 
                foreach (RootObject rO in filteredData) {
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
                                        if (!temp.Contains(r)) {
                                            temp.Add(r);
                                        }
                                    }
                                    if (!(l.PersonCount.Equals(lr.PersonCount))) {
                                        if (!temp.Contains(r)) {
                                            temp.Add(r);
                                        }
                                    }
                                    if (!(l.SignsOfLife.Equals(lr.SignsOfLife))) {
                                        if (!temp.Contains(r)) {
                                            temp.Add(r);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //If there is something in the temp list, then the list filteredData will be cleared first, before the list temp is added. 
            if (!(temp.Count == 0)) {
                filteredData.Clear();
                filteredData.AddRange(temp);
            }

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
            //Console.WriteLine(json);
        }

       /* private static void SendDataWithRabbitMQ(List<Location> locations) {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {
                channel.QueueDeclare(queue: "Consumer_Azure_Queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var message = JsonConvert.SerializeObject(locations);
                var body = Encoding.UTF8.GetBytes(message);

                var properties = channel.CreateBasicProperties();
                properties.Persistent = true;

                channel.BasicPublish(exchange: "",
                                     routingKey: "Consumer_Azure_Queue",
                                     basicProperties: properties,
                                     body: body);
                Console.WriteLine(message);
                Console.WriteLine();
                Console.WriteLine();
            }*/
        }
    }
}
