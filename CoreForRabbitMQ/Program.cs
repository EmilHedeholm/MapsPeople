using System;
using System.Collections.Generic;
using System.Text;
using DataModels;
using DatabaseAccess;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using RabbitMQ.Client.Exceptions;
using System.Xml.Serialization;
using Confluent.Kafka;
using MessageBrokers;
//using RabbitMQ.Client.Exceptions;

namespace CoreForRabbitMQ {
    public class Program {
        static IDataAccess dataAccess { get; set; }
        static IMessageBroker messageBroker { get; set; }
       
        public static void Main(string[] args) {

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
                string messageBrokerChoice = Console.ReadLine();
                switch (messageBrokerChoice) {
                    case "kafka":
                        messageBroker = new MessageBrokerKafka();
                        choice2 = false;
                        break;
                    case "rabbitmq":
                        messageBroker = new MessageBrokerRabbitMQ();
                        choice2 = false;
                        break;
                    default:
                        Console.WriteLine("not a recognized messagebroker, try again");
                        break;
                }
            }

            ReceiveUpdate();
        }


        //This method uses RabbitMQ to get data from the the customer consumers. 
        public static void ReceiveUpdate() {
            messageBroker.ReceiveUpdateFromConsumer(dataAccess);
        }
    }
}
