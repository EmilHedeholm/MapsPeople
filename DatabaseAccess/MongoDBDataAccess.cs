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

        private static MongoClient client = new MongoClient("mongodb://localhost:27017");
        private static IMongoDatabase database = client.GetDatabase("MapsPeople");
        private static IMongoCollection<Location> collection = database.GetCollection<Location>("Locations");
        public void CreateLocation(Location location) {
            collection.InsertOne(location);
            //collection.ReplaceOne(new BsonDocument("_id", location.Id), location, new ReplaceOptions { IsUpsert = true });
        }

        public void DeleteLocationAndSubLocations(string locationId) {
            throw new NotImplementedException();
        }

        public void DeleteLocationsByConsumerId(int consumerid) {
            throw new NotImplementedException();
        }

        public List<Location> GetAllConnectedLocations(string id, List<Location> foundLocations) {
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
            return foundLocations;
        }

        public Location GetLocationByExternalId(string externalId) {
            Location location = null;
            var filter = Builders<Location>.Filter.Eq("ExternalId", externalId);
            if (externalId != null) {
                location = collection.Find(filter).FirstOrDefault();
            }
            return location;
        }

        public Location GetLocationById(string id) {
            var filter = Builders<Location>.Filter.Eq("_id", id);
            return collection.Find(filter).FirstOrDefault();
        }

        public List<Location> GetLocations() {
            var filter = Builders<Location>.Filter.Empty;
            return collection.Find(filter).ToList();
            
        }

        public void UpdateLocation(Location location) {
            collection.ReplaceOne(new BsonDocument("_id", location.Id), location, new ReplaceOptions { IsUpsert = true });
        }
    }
}
