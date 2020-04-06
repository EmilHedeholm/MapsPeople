using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using XMLConsumer.Models;
using DataModels;
using System.Threading;
using RabbitMQ.Client;
using Newtonsoft.Json;

namespace XMLConsumer {
    public class Program {

        static ArrayOfLocation oldData = new ArrayOfLocation();
        public static void Main(string[] args) {
            while (true) {
                //Wait for 3 sek. 
                Thread.Sleep(3000);
                List<DataModels.Location> data = GetData();
                if (!(data.Count == 0)) {
                    SendDataWithRabbitMQ(data);
                }
            }
        }

        private static List<DataModels.Location> GetData() {
            HttpWebRequest req = null;
            HttpWebResponse res = null;

            string url = "http://localhost:51490/Service1.svc/GetXMLData";

            req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "application/xml; charset=utf-8";
            req.Timeout = 30000;
            req.Headers.Add("SOAPAction", url);

            res = (HttpWebResponse)req.GetResponse();
            Stream responseStream = res.GetResponseStream();
            var streamReader = new StreamReader(responseStream);

            var soapResonseXmlDocument = new XmlDocument();
            soapResonseXmlDocument.LoadXml(streamReader.ReadToEnd());

            ArrayOfLocation locations;
            using (TextReader textReader = new StringReader(soapResonseXmlDocument.InnerXml)) {
                using (XmlTextReader reader = new XmlTextReader(textReader)) {
                    XmlSerializer serializer = new XmlSerializer(typeof(ArrayOfLocation));
                    locations = (ArrayOfLocation)serializer.Deserialize(reader);
                }
            }
            ArrayOfLocation filteredData =  FilterData(locations);
            return ConvertToInternalModel(filteredData);
        }

        private static ArrayOfLocation FilterData(ArrayOfLocation newData) {
            ArrayOfLocation filteredData = new ArrayOfLocation();

            if (oldData.Location.Count == 0) {
                oldData.Location.AddRange(newData.Location);
                filteredData.Location.AddRange(newData.Location);
            }

            foreach (var locationNew in newData.Location) {
                foreach (var locationOld in oldData.Location) {
                    if (locationNew.Id.Equals(locationOld.Id)){
                        if (!locationNew.Availability.Equals(locationOld.Availability)) {
                            if (!filteredData.Location.Contains(locationNew)) {
                                filteredData.Location.Add(locationNew);
                            }
                        }
                    }
                }
            }

            ArrayOfLocation temp = new ArrayOfLocation();
            if (!(filteredData.Location.Count == 0)) {
                oldData = newData;
            }
            return filteredData;
        }

        private static List<DataModels.Location> ConvertToInternalModel(ArrayOfLocation filteredData) {
            List<DataModels.Location> convertedLocations = new List<DataModels.Location>();

            foreach (var location in filteredData.Location) {
                DataModels.Location convertedLocation = new DataModels.Location();
                convertedLocation.ConsumerId = 3;
                Source source = new Source();
                source.Id = location.Availability.Id;
                source.Type = "Room Availablility";
                source.TimeStamp = DateTime.Parse(location.Availability.TimeStamp);
                State Available = new State() { Id = "0" + source.Id, Property = "Available", Value = location.Availability.Available };
                source.State.Add(Available);
                convertedLocation.Sources.Add(source);
                convertedLocations.Add(convertedLocation);
            }
            return convertedLocations;
        }

        private static void SendDataWithRabbitMQ(List<DataModels.Location> data) {
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
        }
    }
}
