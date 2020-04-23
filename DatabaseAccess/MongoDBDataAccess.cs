using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace DatabaseAccess {
    public class MongoDBDataAccess : IDataAccess {

        MongoClient client { get; set; }
        IMongoDatabase database { get; set; }
        IMongoCollection<Location> collection { get; set; }

        public MongoDBDataAccess() {
            try {
                client = new MongoClient("mongodb://localhost:27017");
                database = client.GetDatabase("MapsPeople");
                collection = database.GetCollection<Location>("Locations");
            }catch(MongoException me) {
                throw new Exception("Something went wrong when trying to connect to the database", me);
            }

        }
        public void CreateLocation(Location location) {
            try {
                collection.InsertOne(location);
            }catch(MongoException me) {
                throw new Exception("Something went wrong when trying to insert a location", me);
            }
        }

        public void DeleteLocationAndSubLocations(string locationId) {
            throw new NotImplementedException();
        }

        public void DeleteLocationsByConsumerId(int consumerid) {
            throw new NotImplementedException();
        }

        public List<Location> GetAllConnectedLocations(string id, List<Location> foundLocations) {
            try {
                var builder = Builders<Location>.Filter;
                var filter = builder.Or(builder.Eq("_id", id), builder.Eq("ParentId", id));
                var locations = collection.Find(filter).ToList();

                foreach (var location in locations) {
                    if (location.Id.Equals(id)) {
                        foundLocations.Add(location);
                    } else {
                        foundLocations.Add(location);
                        GetAllConnectedLocations(location.Id, foundLocations);
                    }
                }
            }catch(MongoException me) {
                throw new Exception("Something went wrong when trying to get a location", me);
            }
            return foundLocations;
        }

        public Location GetLocationByExternalId(string externalId) {
            Location location = null;
            try { 
            if (externalId != null) {
                var filter = Builders<Location>.Filter.Eq("ExternalId", externalId);
                location = collection.Find(filter).FirstOrDefault();
            }
            } catch (MongoException me) {
                throw new Exception("Something went wrong when trying to get a location", me);
            }
            return location;
        }

        public Location GetLocationById(string id) {
            Location location = null;
            try { 
            if (id != null) {
                var filter = Builders<Location>.Filter.Eq("_id", id);
                location = collection.Find(filter).FirstOrDefault();
            }
            } catch (MongoException me) {
                throw new Exception("Something went wrong when trying to get a location", me);
            }
            return location;
        }

        public List<Location> GetLocations() {
            List<Location> foundLocations = null;
            try { 
            var filter = Builders<Location>.Filter.Empty;
            foundLocations = collection.Find(filter).ToList();
            } catch (MongoException me) {
                throw new Exception("Something went wrong when trying to get a location", me);
            }
            return foundLocations;
        }

        public void UpdateLocation(Location location) {
            try { 
            collection.ReplaceOne(new BsonDocument("_id", location.Id), location, new ReplaceOptions { IsUpsert = true });
            } catch (MongoException me) {
                throw new Exception("Something went wrong when trying to update a location", me);
            }
        }
    }
}
