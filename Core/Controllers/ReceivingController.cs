using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.Models;
using DataModels;
using DatabaseAccess;

namespace Core.Controllers
{
    public class ReceivingController : ApiController {
        IDataAccess dataAccess = new DataAccess();
        //This post method receives location data from consumers and maps them with data from other consumers before
        //savin the changes to database, converting to the external message format and sending it out of the system.
        public HttpResponseMessage Post([FromBody]IEnumerable<Location> locations) {
            var message = Request.CreateResponse(HttpStatusCode.BadRequest);

            foreach (var location in locations) {
                //Getting the location data from the DB via ID.
                Location existingLocation = GetLocationById(location.Id), 
                        completeLocation = null;
                //Checking if the location was found in the DB, if not get it by ExternalId.
                if (existingLocation == null) {
                    //Getting the location data from the DB via externalID.
                    existingLocation = GetLocationByExternalId(location.ExternalId);
                    if (existingLocation == null) {
                        //Going through the mapping table to find the location.
                        existingLocation = FindLocationByMappingTable(location);
                        
                    }
                }
                if (existingLocation != null) {
                    //Combine the data from both location and existingLocation
                    completeLocation = Map(location, existingLocation);
                    UpdateLocation(completeLocation);
                    //TODO: Create converting and sending functionality
                    //var external = ConvertToExternal(location);
                    //SendMessage(external);
                    message = Request.CreateResponse(HttpStatusCode.OK);
                } else {
                    //If the existingLocation is still null, insert it into the database as is.
                    InsertIntoDB(location);
                    message = Request.CreateResponse(HttpStatusCode.Created);
                }
                
            }

            return message;
        }
        //This method maps a locations ConsumerId and Id with data from a list and adds an ExternalId if a match is found.
        private Location FindLocationByMappingTable(Location location) {
            List<MappingEntry> entries = new List<MappingEntry>();
            entries = FillEntries(entries);
            foreach (var entry in entries) {
                //If the location.id and location.ConsumerId matches the entry we set the location.ExternalId to the entry.ExternalId.
                if (location.ConsumerId == entry.ConsumerId && location.Id.Equals(entry.Id)) {
                    location.ExternalId = entry.ExternalId;
                }
            }
            return GetLocationByExternalId(location.ExternalId);
        }

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

        private void UpdateLocation(Location completeLocation) {
            dataAccess.UpdateLocation(completeLocation);
        }

        private Location GetLocationByExternalId(string externalId)
        {
            return dataAccess.GetLocationByExternalId(externalId);
        }
        //This method maps data from a newly received location with data pertaining to that location from the database
        // then it merges them into a complete location, updates the sources and returns the complete location.
        private Location Map(Location location, Location existingLocation) {
            Location completeLocation = existingLocation;
            //Mapping locationId.
            if (completeLocation.Id == null) {
                completeLocation.Id = location.Id;
            }
            //Mapping ExternalId.
            if (completeLocation.ExternalId == null && location.ExternalId != null) {
                completeLocation.ExternalId = location.ExternalId;
            }
            //Mapping Parent.
            if (completeLocation.Parent == null && location.Parent != null) {
                completeLocation.Parent = location.Parent;
            }
            //Inserting new sources.
            if(location.Sources.Count > completeLocation.Sources.Count) {
                foreach (var source in location.Sources) {
                    if (!completeLocation.Sources.Contains(source)){
                        completeLocation.Sources.Add(source);
                    }
                }
            }
            //Updating states
            List<Source> completedSources = new List<Source>();
            foreach (var source in location.Sources) {
                foreach (var completeSource in completeLocation.Sources) {
                    if (source.Id == completeSource.Id && source.TimeStamp < completeSource.TimeStamp) {
                        completedSources.Add(source);
                    } else {
                        completedSources.Add(completeSource);
                    }
                }
            }
            completeLocation.Sources = completedSources;

            return completeLocation;
        }

        private void InsertIntoDB(Location location){
            //return DataBase.insert(location);
            // TODO: Implement this when the db is up and running.
            dataAccess.CreateLocation(location);
        }
        private Location GetLocationById(string id) {
            //return dataBase.GetLocation(id);
            //TODO: Implement this with the DB.
            return dataAccess.GetLocationById(id);
        }
        /*
        private List<ExternalModel> ConvertToExternal(Location location) {
            //TODO: Implement this when the external converter class is finished.
            //ExternalConverter externalConverter = new ExternalConverter();
            //List<ExternalModel> externalModels = externalConverter.Convert(location);
            throw new NotImplementedException();
        }*/

    }
}
