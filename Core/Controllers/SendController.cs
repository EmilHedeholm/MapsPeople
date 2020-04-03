using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.Service;
using DatabaseAccess;
using DataModels;

namespace Core.Controllers
{
    public class SendController : ApiController {
        IDataAccess dataAccess = new DataAccess();

        public IEnumerable<ExternalModel> Get() {
            IEnumerable<Location> allLocations = GetAllLocations();
            List<ExternalModel> messages = new List<ExternalModel>();
            foreach(var location in allLocations) {
                messages.AddRange(Convert(location));
            }
            return messages;
        }

        private List<ExternalModel> Convert(Location location) {
            ExternalConverter externalConverter = new ExternalConverter();
            return externalConverter.Convert(location);
        }

        private List<Location> GetAllLocations() {
            return dataAccess.GetLocations();
        }

    }
    
    
}
