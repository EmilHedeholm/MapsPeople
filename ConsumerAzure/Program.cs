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
                Thread.Sleep(3000);
                List<Location> data = GetData();
                if (!(data.Count == 0)) {
                    SendData(data);
                }
            }
        }

        private static List<Location> GetData() {
            string jsonstr;
            var request = WebRequest.Create("https://mi-ucn-live-data.azurewebsites.net/occupancy?datasetid=6fbb3035c7e2436ba335edac") as HttpWebRequest;
            var response = request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                jsonstr = sr.ReadToEnd();
            }
            List<RootObject> sources = JsonConvert.DeserializeObject<List<RootObject>>(jsonstr);

            FilterData(sources);
            return ConvertFromJsonToInternalModel(filteredData);
        }

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

        private static void FilterData(List<RootObject> rawData) {
            List<RootObject> temp = new List<RootObject>();
            foreach (RootObject r in rawData) {
                foreach (RootObject rO in filteredData) {
                    if (r.Id.Equals(rO.Id)) {
                        foreach (LastReport l in r.LastReports) {
                            foreach (LastReport lr in rO.LastReports) {
                                if (l.Id.Equals(lr.Id)) {
                                    if (!(l.MotionDetected.Equals(lr.MotionDetected))) {
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
            filteredData.Clear();
            filteredData.AddRange(temp);
        }

        private static void SendData(List<Location> locations) {
            var client = new RestClient();
            client.BaseUrl = new Uri("");
            string json = JsonConvert.SerializeObject(locations);
            var request = new RestRequest(Method.POST);
            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            var response = client.Execute(request);
        }
    }
}
