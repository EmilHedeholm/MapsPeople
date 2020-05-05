using DataModels;
using System.Collections.Generic;

namespace DatabaseAccess {
    public interface IDataAccess {
        Location GetLocationById(string id);
        Location GetLocationByExternalId(string externalId);
        HashSet<Location> GetAllConnectedLocations(string id, HashSet<Location> foundLocations);
        void CreateLocation(Location location);
        void UpdateLocation(Location location);
    }
}
