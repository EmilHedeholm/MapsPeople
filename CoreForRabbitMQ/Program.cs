using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using DataModels;
using DatabaseAccess;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using Send;

namespace CoreForRabbitMQ {
    public class Program {
        public static void Main(string[] args) {
            ReceiveDataFromRabbitMQ();
        }

        static IDataAccess dataAccess = new DataAccess();
        //This post method receives location data from consumers and maps them with data from other consumers before
        //savin the changes to database, converting to the external message format and sending it out of the system.
        public static void Receive(IEnumerable<Location> locations) {
            foreach (var location in locations) {
                //Getting the location data from the DB via ID.
                Location existingLocation = GetLocationById(location.Id),
                        completeLocation = null;
                //Checking if the location was found in the DB, if not get it by ExternalId.
                if (existingLocation == null || existingLocation.Id.Equals("0")) {
                    //Getting the location data from the DB via externalID.
                    existingLocation = GetLocationByExternalId(location.ExternalId);
                    if (existingLocation == null || existingLocation.Id.Equals("0")) {
                        //Going through the mapping table to find the location.
                        existingLocation = FindLocationByMappingTable(location);
                    }
                }
                //Checks if the location was found. 
                if (existingLocation != null && !existingLocation.Id.Equals("0")) {
                    //If found then combine the data from both location and existingLocation
                    completeLocation = Map(location, existingLocation);
                    UpdateLocation(completeLocation);
                    Location update = PrepareUpdate(completeLocation, existingLocation);
                    if (update.Sources.Count > 0) {
                        var external = ConvertToExternal(update);
                        SendMessage(external);
                    }
                    
                } else if (location.Id != "0") {
                    //If the existingLocation is still null, insert it into the database as is.
                    InsertIntoDB(location);
                }

            }
        }

        private static Location PrepareUpdate(Location completeLocation, Location existingLocation) {
            Location update = new Location() {
                ConsumerId = completeLocation.ConsumerId,
                ExternalId = completeLocation.ExternalId, 
                Id = completeLocation.Id, 
                ParentId = completeLocation.ParentId
            };
            foreach (var completeSource in completeLocation.Sources) {
                foreach (var existingSource in existingLocation.Sources) {
                        foreach (var completeState in completeSource.State) {
                            foreach (var existingState in existingSource.State) {
                                if (completeState.Equals(existingState)) {
                                    if (!completeState.Value.Equals(existingState.Value)) {
                                        update.Sources.Add(completeSource);
                                    }
                                }
                            }
                        }
                }
            }
            return update;
        }

        private static void SendMessage(List<ExternalModel> external) {
            SendMessage sender = new SendMessage();
            sender.SendUpdate(external);
        }

        //This method maps a locations ConsumerId and Id with data from a list and adds an ExternalId if a match is found.
        private static Location FindLocationByMappingTable(Location location) {
            List<MappingEntry> entries = new List<MappingEntry>();
            entries = FillEntries(entries);
            foreach (var entry in entries) {
                //If the location.id and location.ConsumerId matches the entry we set the location.ExternalId to the entry.ExternalId.
                if (location.ConsumerId == entry.ConsumerId && location.ExternalId.Equals(entry.Id)) {
                    location.ExternalId = entry.ExternalId;
                }
            }
            Location result = GetLocationByExternalId(location.ExternalId);
            if (result != null) {
                foreach (var source in location.Sources) {
                    if (!result.Sources.Contains(source)) {
                        result.Sources.Add(source);
                    }
                } 
            }
            return result;
        }

        //In this method a list of entries, is being filled. In each entry an ID is manually paired with an externalID. 
        private static List<MappingEntry> FillEntries(List<MappingEntry> entries) {
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "1", ExternalId = "GS202" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "2", ExternalId = "GA203" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "3", ExternalId = "GD202" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "4", ExternalId = "F210" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "5", ExternalId = "A203" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "6", ExternalId = "F205" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "7", ExternalId = "B215" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "8", ExternalId = "1.04.01" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "9", ExternalId = "G203" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "10", ExternalId = "GF202" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "11", ExternalId = "S207" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "12", ExternalId = "GF201" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "13", ExternalId = "B205" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "14", ExternalId = "E1" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "15", ExternalId = "GF203" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "16", ExternalId = "GA202" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "17", ExternalId = "B210" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "18", ExternalId = "S203" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "19", ExternalId = "B212" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "20", ExternalId = "D201" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "21", ExternalId = "B201" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "22", ExternalId = "D205" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "23", ExternalId = "1.09.01" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "24", ExternalId = "B226" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "25", ExternalId = "A215" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "26", ExternalId = "B202" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "27", ExternalId = "F220" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "28", ExternalId = "S1" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "29", ExternalId = "GB202" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "30", ExternalId = "1.27.04" });
            return entries;
        }

        //This method updates a location node. 
        private static void UpdateLocation(Location completeLocation) {
            dataAccess.UpdateLocation(completeLocation);
        }

        //This method gets a location from the Database by its external ID. 
        private static Location GetLocationByExternalId(string externalId) {
            return dataAccess.GetLocationByExternalId(externalId);
        }
        //This method maps data from a newly received location with data pertaining to that location from the database
        // then it merges them into a complete location, updates the sources and returns the complete location.
        private static Location Map(Location location, Location existingLocation) {
            Location completeLocation = existingLocation;
            //Mapping locationId.
            if (completeLocation.Id == "0" && location.Id != "0") {
                completeLocation.Id = location.Id;
            }
            //Mapping ExternalId.
            if (completeLocation.ExternalId == "0" && location.ExternalId != "0") {
                completeLocation.ExternalId = location.ExternalId;
            }
            //Mapping Parent.
            if (completeLocation.ParentId == "0" && location.ParentId != "0") {
                completeLocation.ParentId = location.ParentId;
            }
            //Inserting new sources.
            foreach (var source in location.Sources) {
                if (!completeLocation.Sources.Contains(source)) {
                    completeLocation.Sources.Add(source);
                }
            }
            //Updating states
            List<Source> completedSources = new List<Source>();
            foreach (var source in location.Sources) {
                foreach (var completeSource in completeLocation.Sources) {
                    //If it is the same source and the source from location is newer than the completed source, then the source is added to the list of completed sources. 
                    if (source.Id == completeSource.Id && source.TimeStamp > completeSource.TimeStamp) {
                        completedSources.Add(source);
                    } else {
                        completedSources.Add(completeSource);
                    }
                }
            }
            completeLocation.Sources = completedSources;

            return completeLocation;
        }

        //This method creates a location node in the database. 
        private static void InsertIntoDB(Location location) {
            dataAccess.CreateLocation(location);
        }
        //This method gets a location by its ID from the database. 
        private static Location GetLocationById(string id) {
            return dataAccess.GetLocationById(id);
        }

        //This method uses RabbitMQ to get data from the the customer consumers. 
        public static void ReceiveDataFromRabbitMQ() {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {
                channel.QueueDeclare(queue: "Consumer_Queue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                Console.WriteLine(" [*] Waiting for messages.");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, ea) => {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                    if (message != null) {
                        //The message is converted from JSON to IEnumerable<Location>.
                        var deserializedMessage = JsonConvert.DeserializeObject<IEnumerable<Location>>(message);
                        Receive(deserializedMessage);
                    }
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
                channel.BasicConsume(queue: "Consumer_Queue",
                                     autoAck: false,
                                     consumer: consumer);
                Console.ReadLine();
            }
        }
        private static List<ExternalModel> ConvertToExternal(Location location) {
            ExternalConverter externalConverter = new ExternalConverter();
            return externalConverter.Convert(location);
        }
    }
}
