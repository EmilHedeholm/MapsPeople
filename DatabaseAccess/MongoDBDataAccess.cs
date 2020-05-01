using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace DatabaseAccess {
    public class MongoDBDataAccess : IDataAccess {

        MongoClient client { get; set; }
        IMongoDatabase database { get; set; }
        IMongoCollection<Location> collection { get; set; }

        //Initiliazes a MongoClient and specifies the name of the database and the collection it should take from.
        public MongoDBDataAccess() {
            try {
                client = new MongoClient("mongodb://localhost:27017");
                database = client.GetDatabase("MapsPeople");
                collection = database.GetCollection<Location>("Locations");
            }catch(MongoException me) {
                throw new Exception("Something went wrong when trying to connect to the database", me);
            }

        }

        //this method takes the current collection and inserts the location
        //parameter: location
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

        //This method gets a list of all the locations that are connected through a specified Id.
        //parameter: the Id of the location, an empty List of locations 
        //return the list of locations
        public HashSet<Location> GetAllConnectedLocations(string id, HashSet<Location> foundLocations) {
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

        //This method gets a location based on the provided external id.
        //parameter: the externalId of the location
        //return the found location
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

        //This method gets a location based on a specified Id
        //parameter: the Id of the location
        //return the found location
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

        // This method gets a list of all the locations
        //returns all locations
        //public List<Location> GetLocations() {
        //    List<Location> foundLocations = null;
        //    try { 
        //    var filter = Builders<Location>.Filter.Empty;
        //    foundLocations = collection.Find(filter).ToList();
        //    } catch (MongoException me) {
        //        throw new Exception("Something went wrong when trying to get a location", me);
        //    }
        //    return foundLocations;
        //}

        //This method updates a location if the id provided matches one in the database otherwise it inserts it.
        //parameter: location
        public void UpdateLocation(Location location) {
            try {
                if (GetLocationById(location.Id) != null) {
                    List<Source> dbSources = GetSourcesFromLocation(location);
                    foreach (var source in location.Sources) {
                        foreach (var dbSource in dbSources) {
                            if (source.TimeStamp > dbSource.TimeStamp) {
                                collection.ReplaceOne(new BsonDocument("_id", location.Id), location, new ReplaceOptions { IsUpsert = false });
                            }
                        }
                    }
                } else {
                    CreateLocation(location);
                }    
            } catch (MongoException me) {
                throw new Exception("Something went wrong when trying to update a location", me);
            }
        }

        private List<Source> GetSourcesFromLocation(Location location) {
            var filter = Builders<Location>.Filter.Eq("_id", location.Id);
            var project = Builders<Location>.Projection.Include("Sources.$");
            var bsonSources = collection.Find(filter).Project(project).ToList();
            List<Source> foundSources = new List<Source>();
            foreach (var source in bsonSources) {
                Source convertedSource = BsonSerializer.Deserialize<Source>(source);
                foundSources.Add(convertedSource);
            }
            return foundSources;

            
            
        }
    }
}
