using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;
using Neo4jClient;

namespace DatabaseAccess
{
    public class DataAccess : IDataAccess {

        GraphClient client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "1234");
        public void CreateLocation(Location location) {
            //connects to the neo4j database
            client.Connect();

            //creates a location node if one does not exist otherwise nothing happens
            client.Cypher
            .Merge("(location:Location { Id: {id} })")
            .OnCreate()
            .Set("location.ParentId = {parentId}")
            .Set("location.ExternalId = {externalId}")
            .Set("location.ConsumerId = {consumerId}")
            .WithParams(new { id = location.Id, parentId = location.Parent, externalId = location.ExternalId, consumerId = location.ConsumerId })
            .ExecuteWithoutResults();

            //creates a relation to the parent of the location that was just inserted
            //if it does not already exist otherwise nothing happens
            client.Cypher
            .Match("(a:Location)")
            .Where((Location a) => a.Id == location.Id)
            .Match("(b:Location)")
            .Where((Location b) => b.Id == location.Parent)
            .Merge("(a)-[r:Located_In]->(b)")
            .ExecuteWithoutResults();

            //runs through all sources on the location
            foreach (Source source in location.Sources) {
                //checks if the source exists. if it does updates the timestamp 
                var foundSources = client.Cypher
                  .Match("(source: Source { Id: {id}})")
                  .Where("(source)-[:Located_In]->(:Location { Id: {locationId}})")
                 // .Set("source.TimeStamp = {timeStamp}")
                  .WithParams(new { id = source.Id, locationId = location.Id, /*timeStamp = source.TimeStamp*/ })
                  .Return<Source>("source")
                  .Results;

                //if the source does not exist a new one is created
                if (foundSources.Count() == 0) {
                    client.Cypher
                    .Create("(source:Source {Id: {sourceId}})")
                    .Set("source.Type = {sourceType}")
                    .Set("source.TimeStamp = {sourceTimeStamp}")
                    .WithParams(new { sourceId = source.Id, sourceType = source.Type, sourceTimeStamp = source.TimeStamp })
                    .ExecuteWithoutResults();

                    //creates a relation to the location the source belongs to
                    client.Cypher
                    .Match("(a:Source)")
                    .Where((Source a) => a.Id == source.Id)
                    .Match("(b:Location)")
                    .Where((Location b) => b.Id == location.Id)
                    .Create("(a)-[r:Located_In]->(b)")
                    .ExecuteWithoutResults();
                }
                //runs through all the states for a source
                foreach (var state in source.State) {
                    string stateId = location.Id + source.Id;
                    //checks if the state exists. if it does updates the value 
                    var foundStates = client.Cypher
                      .Match("(state: State { Id: {id}})")
                      .Where("(state)-[:State_For]->(:Source { Id: {sourceId}})")
                      //.Set("state.Property = {property}")
                      //.Set("state.Value = {value}")
                      .WithParams(new {
                          id = state.Key,
                          sourceId = source.Id,
                         /* property = state.Key,
                          value = state.Value*/
                      })
                      .Return<KeyValuePair<string, string>>("state")
                      .Results;

                    if (foundStates.Count() == 0) {
                        client.Cypher
                        .Create("(state:State {Id: {id}})")
                        .Set("state.Property = {property}")
                        .Set("state.Value = {value}")
                        .WithParams(new { property = state.Key, value = state.Value, id = stateId })
                        .ExecuteWithoutResults();

                        //creates a relation to the source the source belongs to
                        client.Cypher
                        .Match("(a:State)")
                        .Where("a.Id = {id}")
                        .WithParams(new { id = stateId })
                        .Match("(b:Source)")
                        .Where((Source b) => b.Id == source.Id)
                        .Merge("(a)-[r:State_For]->(b)")
                        .ExecuteWithoutResults();
                    }
                }

            }
        }

        public void DeleteLocationAndSubLocations(string locationId) {
            throw new NotImplementedException();
        }

        public void DeleteLocationsByConsumerId(int consumerid) {
            throw new NotImplementedException();
        }

        public Location GetLocationByExternalId(string externalId) {
            client.Connect();
            Location foundLocation = null;
            var locations = client.Cypher
            .Match("(location:Location {ExternalId:{ExternalId}})")
            .WithParams(new { ExternalId = externalId })
            .Return<Location>("location")
            .Results;

            foreach (var location in locations) {
                foundLocation = location;
            }
            if (foundLocation != null) {
                foundLocation.Sources = GetSourcesByLocation(foundLocation);
            }
            client.Dispose();
            return foundLocation;
        }

        public Location GetLocationById(string id) {
            client.Connect();
            Location foundLocation = null;
            var locations = client.Cypher
            .Match("(location:Location {Id:{Id}})")
            .WithParams(new { Id = id })
            .Return<Location>("location")
            .Results;

            foreach (var location in locations) {
                foundLocation = location;
            }
            if (foundLocation != null) { 
            foundLocation.Sources = GetSourcesByLocation(foundLocation);
            }   
            client.Dispose();
            return foundLocation;
        }

        private List<Source> GetSourcesByLocation(Location location) {
            client.Connect();
            var sources = client.Cypher
            .Match("(source:Source)")
            .Where("(source)-[:Located_In]->(:Location {Id: {locationId}})")
            .WithParams(new { locationId = location.Id })
            .Return<Source>("(source)")
            .Results;

            foreach (var source in sources) {
                source.State = GetStatesBySource(source);
            }
            return (List<Source>)sources;
        }

        private  Dictionary<string, string> GetStatesBySource(Source source) {
            client.Connect();
            var states = client.Cypher
            .Match("(state:State)")
            .Where("(state)-[:State_For]->(:Source {Id: {sourceId}})")
            .WithParams(new { sourceId = source.Id })
            .Return<State>("(state)")
            .Results;

            Dictionary<string, string> foundStates = new Dictionary<string, string>();
            foreach (var state in states) {
                foundStates.Add(state.Property, state.value);
            }
            return foundStates;
        }
        public void UpdateLocation(Location location) {
            client.Connect();
            var foundlocation = client.Cypher
            .Match("(location:Location {Id:{id}})")
            .Set("location.ParentId = {parentId}")
            .Set("location.ConsumerId = {consumerId}")
            .Set("location.ExternalId = {externalId}")
            .WithParams(new { id = location.Id, parentId = location.Parent, consumerId = location.ConsumerId, externalId =location.ExternalId })
            .Return<Location>("(location)")
            .Results;

            if(foundlocation.Count() == 0) {
                foundlocation = client.Cypher
                .Match("(location:Location {ExternalId:{externalId}})")
                .Set("location.ParentId = {parentId}")
                .Set("location.ConsumerId = {consumerId}")
                .Set("location.Id = {id}")
                .WithParams(new { externalId = location.ExternalId, parentId = location.Parent, consumerId = location.ConsumerId, id = location.Id })
                .Return<Location>("(location)")
                .Results;
            }
            if (foundlocation.Count() != 0) {
                foreach (var source in location.Sources) {
                    client.Cypher
                    .Match("(source:Source {Id:{id}})")
                    .Where("(source)-[:Located_In]->(:Location {Id: {locationId}})")
                    .Set("source.TimeStamp = {timeStamp}")
                    .Set("source.Type = {type}")
                    .WithParams(new { id = source.Id, timeStamp = source.TimeStamp, type = source.Type, locationId = location.Id })
                    .ExecuteWithoutResults();

                    foreach (var state in source.State) {
                        string stateId = location.Id + source.Id;
                        client.Cypher
                        .Match("(state:State {Property:{property}})")
                        .Where("(state)-[:State_For]->(:Source {Id: {sourceId}})")
                        .Set("state.Id = {id}")
                        .Set("state.Value = {value}")
                        .WithParams(new { id = stateId, property = state.Key, value = state.Value, sourceId = source.Id})
                        .ExecuteWithoutResults();

                    }
                }
            }
            
        }
    }
}
