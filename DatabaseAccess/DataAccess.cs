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
            client.Connect();

            var query = client.Cypher
                .Match("(l:Location)")
                .Where((Location l) => l.Id == location.Parent)
                .Merge("(location:Location {Id})")
                .WithParam("Id", location.Id)
                .Set("location.ParentId = ParentId")
                .WithParam("ParentId", location.Parent)
                .Set("location.ExternalId = ExternalId")
                .WithParam("ExternalId", location.ExternalId)
                .Set("location.ConsumerId = ConsumerId")
                .WithParam("ConsumerId", location.ConsumerId);

            query.ExecuteWithoutResults();
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
