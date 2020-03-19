using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DataModels;

namespace Core.Controllers
{
    public class ReceivingController : ApiController {
        //IDataBase dataBase = new DataBase();
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
                        existingLocation = FindLocationByMappingTable(location.Id);
                    }
                }
                if (existingLocation != null) {
                    //Combine the data from both location and existingLocation
                    completeLocation = Map(location, existingLocation);
                    UpdateLocation(completeLocation);
                    //TODO: Create converting and sending functionality
                    //var message = ConvertToExternal(location);
                    //SendMessage(message);
                    message = Request.CreateResponse(HttpStatusCode.OK);
                } else {
                    //If the existingLocation is still null, insert it into the database as is.
                    InsertIntoDB(location);
                    message = Request.CreateResponse(HttpStatusCode.Created);
                }
                
            }

            return message;
        }

        private Location FindLocationByMappingTable(string id) {
            throw new NotImplementedException();
        }

        private void UpdateLocation(Location completeLocation) {
            throw new NotImplementedException();
        }

        private Location GetLocationByExternalId(string externalId)
        {
            throw new NotImplementedException();
        }

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

        private bool InsertIntoDB(Location location){
            //return DataBase.insert(location);
            // TODO: Implement this when the db is up and running.
            throw new NotImplementedException();
        }
        private Location GetLocationById(string id) {
            //return dataBase.GetLocation(id);
            //TODO: Implement this with the DB.
            throw new NotImplementedException();
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
