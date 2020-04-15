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

        //makes a graph client with the vuri and the credentials for the database
        GraphClient client = new GraphClient(new Uri("http://localhost:7474/db/data"), "neo4j", "1234");

        //this method creates a location node and a relation to its parent and calls CreateSource to create the sources for the location
        //parameter: the location to be inserted into the database
        public void CreateLocation(Location location) {
            //connects to the neo4j database
            client.Connect();

            //creates a location node and a reltion to its parent if it does not already exists
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
        //this method creates sources relations to the location they belong and calls CreateStates to creates states for ech source
        //parameter: the location where the sources to be inserted into the database belongs to
        private void CreateSources(Location location) {
            client.Connect();
            //iterates through the list of sources on location
            foreach (Source source in location.Sources) {
                //checks if the source exists in the database and sets its properties if it does
                var foundSources = client.Cypher
                  .Match("(source: Source { Type: {type}})")
                  .Where("(source)-[:Located_In]->(:Location { Id: {locationId}})")
                  .Set("source.TimeStamp = {timeStamp}")
                  .WithParams(new { timeStamp = source.TimeStamp, type = source.Type, locationId = location.Id })
                  .Return<Source>("source")
                  .Results;

                //if the source does not exist a new one is created
                if (foundSources.Count() == 0) {
                    client.Cypher
                    .Create("(source:Source { Type: {type}})")
                    .Set("source.TimeStamp = {sourceTimeStamp}")
                    .With("source")
                    .Match("(parent:Location)")
                    .Where((Location parent) => parent.Id == location.Id)
                    .Create("(source)-[r:Located_In]->(parent)")
                    .WithParams(new { type = source.Type, sourceTimeStamp = source.TimeStamp })
                    .ExecuteWithoutResults();

                }
                // the id for the states to be inserted into the database
                CreateStates(source, location);
            }
            client.Dispose();
        }

        //this method creates states and relations to the source they belong to 
        //parameter: the source where the states to be inserted into the database belongs to and the id for all the states
        private void CreateStates(Source source, Location location) {
            client.Connect();
            //runs through all the states for a source
            foreach (var state in source.State) {
                //checks if the state exists and sets its properties if it does
                var foundStates = client.Cypher
                  .Match("(state: State { Property: {property}})")
                  .Where("(state)-[:State_For]->(:Source { Type: {type}})-[:Located_In]->(:Location{Id: {id}})")
                  .Set("state.Value = {value}")
                  .WithParams(new { property = state.Property, value = state.Value, type = source.Type, id = location.Id })
                  .Return<State>("state")
                  .Results;

                //if the state deos not exists it is created
                if (foundStates.Count() == 0) {
                    client.Cypher
                    .Create("(state:State { Property: {property}})")
                    .Set("state.Value = {value}")
                    .With("state")
                    .Match("(parent:Source{Type: {type}})")
                    .Where("(parent)-[:Located_In]->(:Location{Id: {id}})")
                    //.Where((Source parent) => parent.Type == source.Type)
                    .Merge("(state)-[r:State_For]->(parent)")
                    .WithParams(new { property = state.Property, value = state.Value, type = source.Type, id = location.Id })
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

        //this method gets a location from the database based on its externalId
        //parameter: the externalId of the location
        //return the found location
        public Location GetLocationByExternalId(string externalId) {
            client.Connect();
            Location foundLocation = null;
            //checks if the location exists int eh database and returns it if it does
            var locations = client.Cypher
            .Match("(location:Location {ExternalId:{ExternalId}})")
            .WithParams(new { ExternalId = externalId })
            .Return<Location>("location")
            .Results;

            //iterates through returned IEnumrable from the cypher statement and sets the location to foundLocation 
            foreach (var location in locations) {
                foundLocation = location;
            }
            //if a location was found GetSourceByLocation is called to get the sources for the loaction
            if (foundLocation != null) {
                foundLocation.Sources = GetSourcesByLocation(foundLocation);
            }
            client.Dispose();
            return foundLocation;
        }
        //this method gets a location from the database based on its Id
        //parameter: the Id of the location
        //return the found location
        public Location GetLocationById(string id) {
            client.Connect();
            Location foundLocation = null;
            //checks if the location exists int eh database and returns it if it does
            var locations = client.Cypher
            .Match("(location:Location {Id:{Id}})")
            .WithParams(new { Id = id })
            .Return<Location>("location")
            .Results;

            //iterates through returned IEnumrable from the cypher statement and sets the location to foundLocation
            foreach (var location in locations) {
                foundLocation = location;
            }
            //if a location was found GetSourceByLocation is called to get the sources for the loaction
            if (foundLocation != null) { 
            foundLocation.Sources = GetSourcesByLocation(foundLocation);
            }   
            client.Dispose();
            return foundLocation;
        }

        public List<Location> GetAllConnectedLocations(string id, List<Location> foundLocations) {
            client.Connect();
            var locations = client.Cypher
                .Match("(l1: Location)")
                .Where("l1.ParentId = {locationId} OR l1.Id = {locationId}")
                .WithParams(new { locationId = id })
                .Return<Location>("(l1)")
                .Results;

            foreach(var location in locations) {
                if(location.Id == id) {
                    location.Sources = GetSourcesByLocation(location);
                    foundLocations.Add(location);
                } else {
                    location.Sources = GetSourcesByLocation(location);
                    foundLocations.Add(location);
                    GetAllConnectedLocations(location.Id, foundLocations);
                }
            }
            return foundLocations;
        }

        //this method gets all the sources for a location
        //parameter: the location that the sources belog to
        //return: a list of found sources
        private List<Source> GetSourcesByLocation(Location location) {
            client.Connect();
            //checks if the sources exist and if they do they are returned
            var sources = client.Cypher
            .Match("(source:Source)")
            .Where("(source)-[:Located_In]->(:Location {Id: {locationId}})")
            .WithParams(new { locationId = location.Id })
            .Return<Source>("(source)")
            .Results;

            //iterates through the found sources and calls GetStatesBySource to get alle the states for ech source
            foreach (var source in sources) {
                source.State = GetStatesBySource(source);
            }
            client.Dispose();
            return (List<Source>)sources;
        }

        //this method gets all the states for a source in the database
        //parameter: the source that the sta¨tes belog to
        //return: a list of found states
        private  List<State> GetStatesBySource(Source source) {
            client.Connect();
            //checks if the states exists in the database and returns them if they do
            var states = client.Cypher
            .Match("(state:State)")
            .Where("(state)-[:State_For]->(:Source {Type: {type}})")
            .WithParams(new { type = source.Type })
            .Return<State>("(state)")
            .Results;

            client.Dispose();
            return (List<State>)states;
        }

        //this method updates a location and all the sources and states related to it
        //parameter: the location to be updated
        public void UpdateLocation(Location location) {
            client.Connect();
            //checks if a location exists in the database and sets its properties and returns it
            var foundlocation = client.Cypher
             .Match("(location:Location {Id:{id}})")
             .Set("location.ParentId = {parentId}")
             .Set("location.ConsumerId = {consumerId}")
             .Set("location.ExternalId = {externalId}")
             .WithParams(new { id = location.Id, parentId = location.ParentId, consumerId = location.ConsumerId, externalId = location.ExternalId })
             .Return<Location>("(location)")
             .Results;

            //if the location exists in the database CreateSources is called
            if (foundlocation.Count() != 0) {
                CreateSources(location);
            }
            client.Dispose();
        }

        public List<Location> GetLocations() {
            client.Connect();

            var locations = client.Cypher
                .Match("(location:Location)")
                .Return<Location>("location")
                .Results;

            List<Location> completeLocations = new List<Location>(); 
            foreach(var location in locations) {
                var foundLocation = GetLocationById(location.Id);
                completeLocations.Add(foundLocation);
            }
            return completeLocations;
        }
    }
}
