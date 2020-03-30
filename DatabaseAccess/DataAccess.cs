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
            .With("location")
            .Match("(parent:Location)")
            .Where((Location parent) => parent.Id == location.ParentId)
            .Merge("(location)-[r:Located_In]->(parent)")
            .WithParams(new { id = location.Id, parentId = location.ParentId, externalId = location.ExternalId, consumerId = location.ConsumerId })
            .ExecuteWithoutResults();


            CreateSources(location);
            client.Dispose();
        }

        private void CreateSources(Location location) {
            client.Connect();
            foreach (Source source in location.Sources) {
                //checks if the source exists 
                var foundSources = client.Cypher
                  .Match("(source: Source { Id: {id}})")
                  .Where("(source)-[:Located_In]->(:Location { Id: {locationId}})")
                  .Set("source.TimeStamp = {timeStamp}")
                  .Set("source.Type = {type}")
                  .WithParams(new { id = source.Id, timeStamp = source.TimeStamp, type = source.Type, locationId = location.Id })
                  .Return<Source>("source")
                  .Results;

                //if the source does not exist a new one is created
                if (foundSources.Count() == 0) {
                    client.Cypher
                    .Create("(source:Source {Id: {sourceId}})")
                    .Set("source.Type = {sourceType}")
                    .Set("source.TimeStamp = {sourceTimeStamp}")
                    .With("source")
                    .Match("(parent:Location)")
                    .Where((Location parent) => parent.Id == location.Id)
                    .Create("(source)-[r:Located_In]->(parent)")
                    .WithParams(new { sourceId = source.Id, sourceType = source.Type, sourceTimeStamp = source.TimeStamp })
                    .ExecuteWithoutResults();

                }
                string stateId = location.Id + source.Id;
                CreateStates(source, stateId);
            }
            client.Dispose();
        }

        private void CreateStates(Source source, string stateId) {
            client.Connect();
            //runs through all the states for a source
            foreach (var state in source.State) {
                //checks if the state exists
                var foundStates = client.Cypher
                  .Match("(state: State { Property: {property}})")
                  .Where("(state)-[:State_For]->(:Source { Id: {sourceId}})")
                  .Set("state.Id = {id}")
                  .Set("state.Value = {value}")
                  .WithParams(new { id = stateId, property = state.Property, value = state.Value, sourceId = source.Id })
                  .Return<State>("state")
                  .Results;

                if (foundStates.Count() == 0) {
                    client.Cypher
                    .Create("(state:State {Id: {id}})")
                    .Set("state.Property = {property}")
                    .Set("state.Value = {value}")
                    .With("state")
                    .Match("(parent:Source)")
                    .Where((Source parent) => parent.Id == source.Id)
                    .Merge("(state)-[r:State_For]->(parent)")
                    .WithParams(new { property = state.Property, value = state.Value, id = stateId })
                    .ExecuteWithoutResults();
                }
            }
            client.Dispose();
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
            client.Dispose();
            return (List<Source>)sources;
        }

        private  List<State> GetStatesBySource(Source source) {
            client.Connect();
            var states = client.Cypher
            .Match("(state:State)")
            .Where("(state)-[:State_For]->(:Source {Id: {sourceId}})")
            .WithParams(new { sourceId = source.Id })
            .Return<State>("(state)")
            .Results;

            client.Dispose();
            return (List<State>)states;
        }
        public void UpdateLocation(Location location) {
            client.Connect();
            var foundlocation = client.Cypher
             .Match("(location:Location {Id:{id}})")
             .Set("location.ParentId = {parentId}")
             .Set("location.ConsumerId = {consumerId}")
             .Set("location.ExternalId = {externalId}")
             .WithParams(new { id = location.Id, parentId = location.ParentId, consumerId = location.ConsumerId, externalId = location.ExternalId })
             .Return<Location>("(location)")
             .Results;

            if (foundlocation.Count() != 0) {
                CreateSources(location);
            }
            client.Dispose();
        }
    }
}
