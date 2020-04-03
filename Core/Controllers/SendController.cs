using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Core.Service;
using DatabaseAccess;
using DataModels;

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
            foreach (var item in messages) {
                if (item.Source != null) {
                    Console.WriteLine(item.Source.Id);
                }
            }
            return messages;
        }

        private List<ExternalModel> Convert(Location location) {
            ExternalConverter externalConverter = new ExternalConverter();
            return externalConverter.Convert(location);
        }

        private List<Location> GetLocation(string id) {
            return dataAccess.GetAllConnectedLocations(id);
        }

    }
    
    
}
