using System;
using System.Collections.Generic;
using System.Text;
using DataModels;
using DatabaseAccess;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using Send;
using ExternalConverter;
using RabbitMQ.Client.Exceptions;
using System.Xml.Serialization;
using Confluent.Kafka;
//using RabbitMQ.Client.Exceptions;

namespace CoreForRabbitMQ {
    public class Program {
        static IDataAccess dataAccess { get; set; }
        static string messageBroker { get; set; }
        static HashSet<string> createdKafkaTopics { get; set; }
        public static void Main(string[] args) {
            //A list of the topics we already created on kafka. This improves performance.
            createdKafkaTopics = new HashSet<string>();
            //there are three implemented databases and the while loop lets you choose which one to use when running the program
            var choice = true;
            while (choice) {
                Console.WriteLine("input the name of the database you want to use(neo4j, mongodb, mssql)");
                var database = Console.ReadLine();
                switch (database) {
                    case "neo4j":
                        dataAccess = new Neo4jDataAccess();
                        choice = false;
                        break;
                    case "mongodb":
                        dataAccess = new MongoDBDataAccess();
                        choice = false;
                        break;
                    case "mssql":
                        dataAccess = new SQLDataAccess();
                        choice = false;
                        break;
                    default:
                        Console.WriteLine("not a recognized database, try again");
                        break;
                }
            }
            var choice2 = true;
            while (choice2) {
                Console.WriteLine("input the name of the messagebroker you want to use(kafka, rabbitmq)");
                messageBroker = Console.ReadLine();
                switch (messageBroker) {
                    case "kafka":
                        ReceiveDataFromKafka(dataAccess);
                        choice2 = false;
                        break;
                    case "rabbitmq":
                        ReceiveDataFromRabbitMQ(dataAccess);
                        choice2 = false;
                        break;
                    default:
                        Console.WriteLine("not a recognized messagebroker, try again");
                        break;
                }
            }
        }

        


        //This post method receives location data from consumers and maps them with data from other consumers before
        //savin the changes to database, converting to the external message format and sending it out of the system.
        public static void Receive(IEnumerable<Location> locations, IDataAccess db) {
            foreach (var location in locations) {
                Location existingLocation = null, update = null; 
                //Getting the location data from the DB via ID.
                try {
                    existingLocation = GetLocationById(location.Id);
                            
                }catch(Exception e) {
                    Console.WriteLine(e.Message);
                }
                //Checking if the location was found in the DB, if not get it by ExternalId.
                if (existingLocation == null || existingLocation.Id.Equals("0")) {
                    //Getting the location data from the DB via externalID.
                    try {
                        existingLocation = GetLocationByExternalId(location.ExternalId);
                    }catch(Exception e) {
                        Console.WriteLine(e.Message);
                    }
                    if (existingLocation == null || existingLocation.Id.Equals("0")) {
                        //Going through the mapping table to find the location.
                        existingLocation = FindLocationByMappingTable(location);
                    }
                }
                //Checks if the location was found. 
                if (existingLocation != null && !existingLocation.Id.Equals("0")) {
                    //If found then combine the data from both location and existingLocation
                    update = Map(location, existingLocation);
                    try {
                        UpdateLocation(update);
                    }catch(Exception e) {
                        Console.WriteLine(e.Message);
                    }
                    //Location update = PrepareUpdate(completeLocation, existingLocation);
                    if (update.Sources.Count > 0) {
                        var external = ConvertToExternal(update, db);
                        if (messageBroker.Equals("rabbitmq")) {
                            SendMessage(external);
                        } else if(messageBroker.Equals("kafka")){
                            SendWithKafka(external);
                        }
                    }
                    
                } else if (location.Id != "0") {
                    //If the existingLocation is still null, insert it into the database as is.
                    try {
                        InsertIntoDB(location);
                    }catch(Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }

            }
        }

        private static void SendMessage(List<ExternalModel> external) {
            SendMessage sender = new SendMessage();
            sender.SendUpdate(external);
        }

        private static void SendWithKafka(List<ExternalModel> external) {
            SendMessage sender = new SendMessage();
            sender.SendUpdateWithKafka(external, createdKafkaTopics);
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
            Location update = new Location() { ConsumerId = existingLocation.ConsumerId, 
                                                         ExternalId = existingLocation.ExternalId, 
                                                         Id = existingLocation.Id, 
                                                         ParentId = existingLocation.ParentId,
                                                         Sources = new List<Source>(location.Sources)};
            //Mapping locationId.
            if (update.Id == "0" && location.Id != "0") {
                update.Id = location.Id;
            }
            //Mapping ExternalId.
            if (update.ExternalId == "0" && location.ExternalId != "0") {
                update.ExternalId = location.ExternalId;
            }
            //Mapping Parent.
            if (update.ParentId == "0" && location.ParentId != "0") {
                update.ParentId = location.ParentId;
            }
           
            return update;
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
        public static void ReceiveDataFromRabbitMQ(IDataAccess db) {
            try {
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
                            Receive(deserializedMessage, db);
                        }
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    };
                    channel.BasicConsume(queue: "Consumer_Queue",
                                         autoAck: false,
                                         consumer: consumer);
                    Console.ReadLine();
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

        private static void ReceiveDataFromKafka(IDataAccess dataAccess) {
            var topic = "Consumer_Topic";
            using (var consumer = new ConsumerBuilder<Ignore, string>(new ConsumerConfig { BootstrapServers = "localhost", GroupId ="Core_Consumer" }).Build()) {
                consumer.Subscribe(topic);
                while (true) {
                    var result = consumer.Consume();
                    var json = JsonConvert.DeserializeObject<List<Location>>(result.Message.Value);
                    Receive(json, dataAccess);
                }
            }
        }

        //this method calls the external converter 
        //parameters: location is the location to be converted, db is the database to use
        private static List<ExternalModel> ConvertToExternal(Location location, IDataAccess db) {
            Converter externalConverter = new Converter();
            return externalConverter.Convert(location, db);
        }
    }
}
