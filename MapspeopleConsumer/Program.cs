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
                List<Location> data = GetData();
                foreach (Location l in data) {
                    Console.WriteLine(l.Id);
                }
                Location l1 = new Location() {
                    ExternalId = "S208",
                    Parent = "D34",
                    Id = "23455"

                };
                List<Location> locations = new List<Location>();
                locations.Add(l1);
                SendData(locations);
            }
        }

        //This method request a token using a post Request using your credentials from Mapspeoples CMS, which gives you a token that you
        //pass along when interacting with Mapspeoples systems
        //Return Security Token
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

        //This method gets data from the geodata source provided by MapsPeople and has been converted to Internal Data Model by using the method ConvertFromJsonToInternalModel. 
        //Return: Is a list of locations in the internal Data model format. 
        private static List<Location> GetData() {
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
            //Console.ReadLine();

            var geodataRequest = new RestRequest($"/{datasetId}/api/geodata/", Method.GET);
            geodataRequest.AddHeader("authorization", response.token_type + " " + response.access_token);
            var geodataResponse = client.Execute(geodataRequest);
            jsonstr = geodataResponse.Content;
            Console.WriteLine(jsonstr);
            Console.ReadLine();
           
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
                location.Sources = new List<Source>();
                location.Id = r.id;
                location.ExternalId = r.externalId;
                location.Parent = r.parentId;
                
                locations.Add(location);
            }
            return (locations);
        }

        //This method sends data to the MapsPeople.
        //Param: Is a list of locations
        private static void SendData(List<Location> locations) {
            List<RootObject> rootObjects = ConvertFromInternalModelToGeodata(locations);
            var client = new RestClient();
            var response = testMethod(client);

            client.BaseUrl = new Uri("https://integration.mapsindoors.com");

            var testRequest = new RestRequest("/api/dataset/", Method.GET);

            testRequest.AddHeader("authorization", response.token_type + " " + response.access_token);

            var something = client.Execute(testRequest);
            string datasetJsonstr = something.Content;

            List<Dataset> datasets = JsonConvert.DeserializeObject<List<Dataset>>(datasetJsonstr);
            string datasetId = datasets[0].Id;
            string json = JsonConvert.SerializeObject(rootObjects);
            //Console.WriteLine(json);
            //Console.ReadLine();
            var postRequest = new RestRequest($"/{datasetId}/api/geodata/", Method.POST);
            postRequest.AddHeader("authorization", response.token_type + " " + response.access_token);
            postRequest.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);
            postRequest.RequestFormat = DataFormat.Json;
            var postResponse = client.Execute(postRequest);
            //Console.WriteLine(json);
            //Console.ReadLine();

        }

        //This method Converts data from the Internal Datamodel format to RootObject objects. 
        //Param: a list of of locations (internal)
        //Return: A list of RootObject objects. 
        private static List<RootObject> ConvertFromInternalModelToGeodata(List<Location> locations) {
            List<RootObject> rootObjects = new List<RootObject>();
            foreach (Location l in locations) {
                RootObject rootObject = new RootObject();
                rootObject.id = l.Id;
                rootObject.externalId = l.ExternalId;
                rootObject.parentId = l.Parent;
                rootObjects.Add(rootObject);
            }
            return (rootObjects);

        }
    }
}
