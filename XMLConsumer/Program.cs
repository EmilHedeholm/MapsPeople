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
using RabbitMQ.Client.Exceptions;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using MessageBrokers;

namespace XMLConsumer {
    public class Program {

        static ArrayOfLocation oldData = new ArrayOfLocation();
        static IMessageBroker messageBroker { get; set; }
        public static void Main(string[] args) {
            var choice = true;
            while (choice) {
                Console.WriteLine("input the name of the messagebroker you want to use(kafka, rabbitmq)");
                var messageBrokerChoice = Console.ReadLine();
                switch (messageBrokerChoice) {
                    case "kafka":
                        messageBroker = new MessageBrokerKafka();
                        choice = false;
                        break;
                    case "rabbitmq":
                        messageBroker = new MessageBrokerRabbitMQ();
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
                    SendUpdate(data);
                }
            }
        }

        //This method gets data from the data provided by XMLSource, and uses the method FilterData on that data. After that it returns a list of filtered data that has been converted to Internal Data Model by using the method ConvertFromJsonToInternalModel. 
        //Return: Is a list of locations in the internal Data model format. 
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

        //This method filters data so that it only keeps data that has been changed. 
        //Param: a ArrayOfLocation object.
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

        //This method Converts data from the deserialized XML to the Internal Datamodel format. 
        //Param: a ArrayOfLocation object. 
        //Return: A list of locations (internal)   
        private static List<DataModels.Location> ConvertToInternalModel(ArrayOfLocation filteredData) {
            List<DataModels.Location> convertedLocations = new List<DataModels.Location>();

            foreach (var location in filteredData.Location) {
                DataModels.Location convertedLocation = new DataModels.Location();
                convertedLocation.ExternalId = location.Id;
                convertedLocation.ConsumerId = 3;
                Source source = new Source();
                source.Type = "Room Availablility";
                source.TimeStamp = DateTime.Parse(location.Availability.TimeStamp);
                State Available = new State() { Property = "Available", Value = location.Availability.Available };
                source.State.Add(Available);
                convertedLocation.Sources.Add(source);
                convertedLocations.Add(convertedLocation);
            }
            return convertedLocations;
        }

        //This method sends data to the Core Controller for RabbitMQ. 
        //Param: Is a list of locations. 
        private static void SendUpdate(List<DataModels.Location> data) {
            messageBroker.SendUpdateToCore(data);
        }
    }
}
