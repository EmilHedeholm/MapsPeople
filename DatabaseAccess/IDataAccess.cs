using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess {
    interface IDataAccess {
        Location GetLocationById(string id);
        Location GetLocationByExternalId(string externalId);
        void CreateLocation(Location location);
        void UpdateSource(string locationId, Source source);
        void DeleteLocationsByConsumerId(int consumerid);
        void DeleteLocationAndSubLocations(string locationId);
    }
}
