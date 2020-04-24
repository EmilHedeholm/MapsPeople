using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseAccess {
    public interface IDataAccess {
        Location GetLocationById(string id);
        Location GetLocationByExternalId(string externalId);
        HashSet<Location> GetAllConnectedLocations(string id, HashSet<Location> foundLocations);
        List<Location> GetLocations();
        void CreateLocation(Location location);
        void UpdateLocation(Location location);
        void DeleteLocationsByConsumerId(int consumerid);
        void DeleteLocationAndSubLocations(string locationId);
    }
}
