using DataModels;
using MapspeopleConsumer.TokenModel;
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

                var client = new RestClient();

                var response = testMethod(client);

                client.BaseUrl = new Uri("https://integration.mapsindoors.com");

                var testRequest = new RestRequest("/api/dataset/", Method.GET);

                testRequest.AddHeader("authorization", response.token_type + " " + response.access_token);

                var something = client.Execute(testRequest);


                Console.WriteLine(something.Content);
                Console.ReadLine();

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

        private static List<Location> GetData() {
            string jsonstr;
            var request = WebRequest.Create("https://mi-ucn-live-data.azurewebsites.net/occupancy?datasetid=6fbb3035c7e2436ba335edac") as HttpWebRequest;
            var response = request.GetResponse();
            using (StreamReader sr = new StreamReader(response.GetResponseStream())) {
                jsonstr = sr.ReadToEnd();
            }
            List<RootObject> sources = JsonConvert.DeserializeObject<List<RootObject>>(jsonstr);

            return ConvertFromJsonToInternalModel(sources);
        }
    }
}
