using DataModels;
using MapspeopleConsumer.JsonModel;
using MapspeopleConsumer.TokenModel;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MapspeopleConsumer {
    class Program {
        static void Main(string[] args) {
            while (true) {
                Thread.Sleep(3000);
                string data = GetData();
               // foreach (Location l in data) {
                    //Console.WriteLine(l.Id);
                //}

                //var client = new RestClient();

                //var response = testMethod(client);

                //client.BaseUrl = new Uri("https://integration.mapsindoors.com");

                //var testRequest = new RestRequest("/api/dataset/", Method.GET);

                //testRequest.AddHeader("authorization", response.token_type + " " + response.access_token);

                //var something = client.Execute(testRequest);


                //Console.WriteLine(data);
                //Console.ReadLine();

            }
        }
        public static Token testMethod(RestClient client)
        {
            client.BaseUrl = new Uri("https://auth.mapsindoors.com/connect/token");

            var request = new RestRequest(Method.POST);

            string encodedBody = string.Format("grant_type=password&client_id=client&username=1061951@ucn.dk&password=T40M51zt4MF0f9NV");

            request.AddParameter("application/x-www-form-urlencoded", encodedBody, ParameterType.RequestBody);
            request.AddParameter("Content-Type", "application/x-www-form-urlencoded", ParameterType.HttpHeader);

            var response = client.Execute<Token>(request);

            return response.Data;
        }

        private static string GetData() {
            string jsonstr;
            //var request = WebRequest.Create("https://integration.mapsindoors.com") as HttpWebRequest;        
            //var response = request.GetResponse();
            var client = new RestClient();

            var response = testMethod(client);

            client.BaseUrl = new Uri("https://integration.mapsindoors.com");

            var testRequest = new RestRequest("/api/dataset/", Method.GET);

            testRequest.AddHeader("authorization", response.token_type + " " + response.access_token);

            var something = client.Execute(testRequest);
            string datasetJsonstr = something.Content;
           
            List<Dataset> datasets= JsonConvert.DeserializeObject<List<Dataset>>(datasetJsonstr);
            string datasetId = datasets[0].Id;
            //Console.WriteLine(datasetId);
            // Console.ReadLine();
            
            var geodataRequest = new RestRequest("/{datasetId}/api/geodata/", Method.GET);
            geodataRequest.AddHeader("authorization", response.token_type + " " + response.access_token);
            var geodataResponse = client.Execute(geodataRequest);
            jsonstr = geodataResponse.Content;
            Console.WriteLine(jsonstr);
            Console.ReadLine();
            //using (StreamReader sr = new StreamReader(something.GetResponseStream())) {
            //jsonstr = sr.ReadToEnd();
            //}
            //List<Dataset> sources = JsonConvert.DeserializeObject<List<Dataset>>(jsonstr);

            return jsonstr;
        }

        //private static List<Location> ConvertFromJsonToInternalModel(List<Dataset> sources) {
        //    List<Location> locations = new List<Location>();
        //    foreach (Dataset d in sources) {
        //        Location location = new Location();
        //        location.Id = d.Id;
        //        location.Sources = new List<Source>();
        //        foreach (LastReport lr in r.lastReports) {
        //            Source source = new Source();
        //            State state = new State();
        //            state.MotionDetected = lr.motionDetected;
        //            state.PersonCount = lr.personCount;
        //            state.SignsOfLife = state.SignsOfLife;
        //            string json = JsonConvert.SerializeObject(state);
        //            source.Id = lr.id;
        //            source.Type = "Occupancy";
        //            //source.State = json;
        //            source.TimeStamp = lr.timeStamp;
        //            location.Sources.Add(source);
        //        }
        //        locations.Add(location);
        //    }
        //    return (locations);
        //}
    }
}
