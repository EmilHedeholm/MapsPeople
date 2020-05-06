using DataModels;
using System.Collections.Generic;

namespace DatabaseAccess {
    //This interface defines the methods that is used in the three other databaseAccess classes. 
    public interface IDataAccess {
        Location GetLocationById(string id);
        Location GetLocationByExternalId(string externalId);
        HashSet<Location> GetAllConnectedLocations(string id, HashSet<Location> foundLocations);
        void CreateLocation(Location location);
        void UpdateLocation(Location location);
    }
}
