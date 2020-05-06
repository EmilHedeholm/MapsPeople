using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DatabaseAccess;
using DataModels;
using ExternalConverter;

namespace Core.Controllers { 
    public class SendController : ApiController {
        IDataAccess DataAccess { get; set; }

        public HttpResponseMessage Get(string id, string database) {
            HttpResponseMessage response = new HttpResponseMessage();
                switch (database) {
                    case "neo4j":
                        DataAccess = new Neo4jDataAccess();
                        response.StatusCode = HttpStatusCode.OK;
                        break;
                    case "mongodb":
                        DataAccess = new MongoDBDataAccess();
                        response.StatusCode = HttpStatusCode.OK;
                        break;
                    case "mssql":
                        DataAccess = new SQLDataAccess();
                        response.StatusCode = HttpStatusCode.OK;
                        break;
                    default:
                        response.StatusCode = HttpStatusCode.BadRequest;
                        break;
            }
            if (response.StatusCode == HttpStatusCode.OK) {
                HashSet<Location> locations = GetLocation(id);
                List<Message> messages = new List<Message>();
                foreach (var location in locations) {
                    messages.AddRange(Convert(location));
                }
                return Request.CreateResponse(response.StatusCode, messages);
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }

        private List<Message> Convert(Location location) {
            Converter externalConverter = new Converter();
            return externalConverter.Convert(location, DataAccess);
        }

        private HashSet<Location> GetLocation(string id) {
            HashSet<Location> foundLocations = new HashSet<Location>();
            foundLocations = DataAccess.GetAllConnectedLocations(id, foundLocations);
            return foundLocations;
        }
    }    
}
