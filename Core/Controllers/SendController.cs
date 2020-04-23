using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using DatabaseAccess;
using DataModels;
using ExternalConverter;

namespace Core.Controllers
{
    public class SendController : ApiController {
        IDataAccess dataAccess = new DataAccess();

        public IEnumerable<ExternalModel> Get(string id) {
            List<Location> locations = GetLocation(id);
            List<ExternalModel> messages = new List<ExternalModel>();
            foreach (var location in locations) {
                messages.AddRange(Convert(location));
            }
            return messages;
        }

        private List<ExternalModel> Convert(Location location) {
            Converter externalConverter = new Converter();
            return externalConverter.Convert(location);
        }

        private List<Location> GetLocation(string id) {
            List<Location> foundLocations = new List<Location>();
            foundLocations = dataAccess.GetAllConnectedLocations(id, foundLocations);
            return foundLocations;
        }

    }
    
    
}
