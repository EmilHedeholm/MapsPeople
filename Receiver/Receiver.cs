using DatabaseAccess;
using DataModels;
using ExternalConverter;
using System;
using System.Collections.Generic;

namespace Receiver {
    //This class receives data from the core, and maps the data with the existing data from MapsPeople CMS.
    public class Receiver {
        public List<Message> Receive(IEnumerable<Location> locations, IDataAccess db) {
            var external = new List<Message>();
            foreach (var location in locations) {
                Location existingLocation = FindExistingLocation(location, db), update = null;
               
                //Checks if the location was found. 
                if (existingLocation != null && !existingLocation.Id.Equals("0")) {
                    //If found then combine the data from both location and existingLocation
                    update = Map(location, existingLocation);
                    try {
                        UpdateLocation(update, db);
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                    //Location update = PrepareUpdate(completeLocation, existingLocation);
                    if (update.Sources.Count > 0) {
                        external.AddRange(ConvertToExternal(update, db));
                    }

                } else if (location.Id != "0") {
                    //If the existingLocation is still null, insert it into the database as is.
                    try {
                        InsertIntoDB(location, db);
                    } catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                }
            }
            return external;
        }

        private Location FindExistingLocation(Location location, IDataAccess db) {
            Location existingLocation = null;
            //Getting the location data from the DB via ID.
            try {
                existingLocation = GetLocationById(location.Id, db);

            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            //Checking if the location was found in the DB, if not get it by ExternalId.
            if (existingLocation == null || existingLocation.Id.Equals("0")) {
                //Getting the location data from the DB via externalID.
                try {
                    existingLocation = GetLocationByExternalId(location.ExternalId, db);
                } catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
                if (existingLocation == null || existingLocation.Id.Equals("0")) {
                    //Going through the mapping table to find the location.
                    existingLocation = FindLocationByMappingTable(location, db);
                }
            }
            return existingLocation;
        }

        //This method maps a locations ConsumerId and Id with data from a list and adds an ExternalId if a match is found.
        private Location FindLocationByMappingTable(Location location, IDataAccess db) {
            List<MappingEntry> entries = new List<MappingEntry>();
            entries = FillEntries(entries);
            foreach (var entry in entries) {
                //If the location.id and location.ConsumerId matches the entry we set the location.ExternalId to the entry.ExternalId.
                if (location.ConsumerId == entry.ConsumerId && location.ExternalId.Equals(entry.Id)) {
                    location.ExternalId = entry.ExternalId;
                }
            }
            Location result = GetLocationByExternalId(location.ExternalId, db);
            if (result != null) {
                foreach (var source in location.Sources) {
                    if (!result.Sources.Contains(source)) {
                        result.Sources.Add(source);
                    }
                }
            }
            return result;
        }

        //In this method a list of entries, is being filled. In each entry an ID is manually paired with an externalID. 
        private List<MappingEntry> FillEntries(List<MappingEntry> entries) {
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "1", ExternalId = "GS202" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "2", ExternalId = "GA203" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "3", ExternalId = "GD202" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "4", ExternalId = "F210" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "5", ExternalId = "A203" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "6", ExternalId = "F205" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "7", ExternalId = "B215" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "8", ExternalId = "1.04.01" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "9", ExternalId = "G203" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "10", ExternalId = "GF202" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "11", ExternalId = "S207" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "12", ExternalId = "GF201" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "13", ExternalId = "B205" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "14", ExternalId = "E1" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "15", ExternalId = "GF203" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "16", ExternalId = "GA202" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "17", ExternalId = "B210" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "18", ExternalId = "S203" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "19", ExternalId = "B212" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "20", ExternalId = "D201" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "21", ExternalId = "B201" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "22", ExternalId = "D205" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "23", ExternalId = "1.09.01" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "24", ExternalId = "B226" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "25", ExternalId = "A215" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "26", ExternalId = "B202" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "27", ExternalId = "F220" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "28", ExternalId = "S1" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "29", ExternalId = "GB202" });
            entries.Add(new MappingEntry { ConsumerId = 3, Id = "30", ExternalId = "1.27.04" });
            return entries;
        }

        //This method updates a location node. 
        private void UpdateLocation(Location completeLocation, IDataAccess db) {
            db.UpdateLocation(completeLocation);
        }

        //This method gets a location from the Database by its external ID. 
        private Location GetLocationByExternalId(string externalId, IDataAccess db) {
            return db.GetLocationByExternalId(externalId);
        }
        //This method maps data from a newly received location with data pertaining to that location from the database
        // then it merges them into a complete location, updates the sources and returns the complete location.
        private Location Map(Location location, Location existingLocation) {
            Location update = new Location() {
                ConsumerId = existingLocation.ConsumerId,
                ExternalId = existingLocation.ExternalId,
                Id = existingLocation.Id,
                ParentId = existingLocation.ParentId,
                Sources = new List<Source>(location.Sources)
            };
            //Mapping locationId.
            if (update.Id == "0" && location.Id != "0") {
                update.Id = location.Id;
            }
            //Mapping ExternalId.
            if (update.ExternalId == "0" && location.ExternalId != "0") {
                update.ExternalId = location.ExternalId;
            }
            //Mapping Parent.
            if (update.ParentId == "0" && location.ParentId != "0") {
                update.ParentId = location.ParentId;
            }

            return update;
        }

        //This method creates a location node in the database. 
        private void InsertIntoDB(Location location, IDataAccess db) {
            db.CreateLocation(location);
        }
        //This method gets a location by its ID from the database. 
        private Location GetLocationById(string id, IDataAccess db) {
            return db.GetLocationById(id);
        }

        //this method calls the external converter 
        //parameters: location is the location to be converted, db is the database to use
        private List<Message> ConvertToExternal(Location location, IDataAccess db) {
            Converter externalConverter = new Converter();
            return externalConverter.Convert(location, db);
        }
    }
}
