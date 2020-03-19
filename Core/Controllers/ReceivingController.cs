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
                    //If the existingLocation is still null, insert it into the database as is.
                    if (existingLocation == null) {
                        InsertIntoDB(location);
                        message = Request.CreateResponse(HttpStatusCode.Created);
                    } else {
                        //Combine the data from both location and existingLocation
                        completeLocation = Map(location, existingLocation);
                        message = Request.CreateResponse(HttpStatusCode.OK);
                    }
                }

                
                
            }

            return message;
        }

        private Location GetLocationByExternalId(string externalId)
        {
            throw new NotImplementedException();
        }

        private Location Map(Location location, Location existingLocation) {
            if (location.Id != existingLocation.Id) {
                //DO stuff here when the table is implemented.
            }
            if (location.Parent == null) {
                location.Parent = existingLocation.Parent;
            }
            foreach (var source in location.Sources) {
                foreach (var existingSource in existingLocation.Sources) {
                    if (source.Id == existingSource.Id) {
                        foreach (var state in source.State) {
                            foreach (var existingState in existingSource.State) {
                                if (state.Key.Equals(existingState.Key)) {
                                    if (!state.Value.Equals(existingState.Value) && source.TimeStamp < existingSource.TimeStamp) {
                                        existingSource.State[state.Key] = state.Value;
                                    }

                                }
                            }
                        }
                    }
                }
            }
            return existingLocation;
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
