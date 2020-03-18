using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using ConsumerAzure.JsonModel;
using DataModels;
using Newtonsoft.Json;
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
            return ConvertFromJsonToInternalModel(filteredData);
        }

        //This method Converts data in JSON format to the Internal Datamodel format. 
        //Param: a list of json objects. 
        //Return: A list of locations (internal)   
        private static List<Location> ConvertFromJsonToInternalModel(List<RootObject> sources) {
            List<Location> locations = new List<Location>();
            foreach (RootObject r in sources) {
                Location location = new Location();
                location.Id = r.SpaceRefId;
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
                    location.Sources.Add(source);
                }
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
            foreach (RootObject r in rawData) {
                foreach (RootObject rO in filteredData) {
                    //Checks that it is the same source. 
                    if (r.Id.Equals(rO.Id)) {
                        foreach (LastReport l in r.LastReports) {
                            foreach (LastReport lr in rO.LastReports) {
                                //Checks that it is the same source. 
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
            //TODO: 
            client.BaseUrl = new Uri("");
            string json = JsonConvert.SerializeObject(locations);
            var request = new RestRequest(Method.POST);
            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            var response = client.Execute(request);
            Console.WriteLine(json);
        }
    }
}
