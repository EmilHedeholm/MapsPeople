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
                  .Set("source.TimeStamp = {timeStamp}")
                  .WithParams(new { id = source.Id, locationId = location.Id, timeStamp = source.TimeStamp })
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
                      .Match("(state: State { Property: {property}})")
                      .Where("(state)-[:State_For]->(:Source { Id: {sourceId}})")
                      .Set("state.Value = {value}")
                      .WithParams(new {
                          property = state.Key,
                          sourceId = source.Id,
                          value = state.Value
                      })
                      .Return<KeyValuePair<string, string>>("state")
                      .Results;

                    if (foundStates.Count() == 0) {
                        client.Cypher
                        .Create("(state:State {Property: {property}})")
                        .Set("state.Value = {value}")
                        .Set("state.Id = {id}")
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
            throw new NotImplementedException();
        }

        public Location GetLocationById(string id) {
            throw new NotImplementedException();
        }

        public void UpdateSource(string locationId, Source source) {
            throw new NotImplementedException();
        }
    }
}
