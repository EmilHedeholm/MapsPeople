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
                Location existingLocation = GetLocation(location.Id), completeLocation = null;
                if (existingLocation == null) {
                    if (location.Id != null) {
                        InsertIntoDB(location);
                        message = Request.CreateResponse(HttpStatusCode.Created);
                    } 
                } else {
                    completeLocation = Map(location, existingLocation);
                    message = Request.CreateResponse(HttpStatusCode.OK);
                }
            }

            return message;
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
                                if (state.key == existingState.key) {
                                    if (state.value =! existingState.value) {
                                        existingState.value = state.value;
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
        private Location GetLocation(string id) {
            //return dataBase.GetLocation(id);
            //TODO: Implement this with the DB.

            // This is temporary for development purposes, delete when Database has been created.
            List<string> states = new List<string> { "18", "77" };
            Source source = new Source { Id = "source 1", State = states, TimeStamp = DateTime.Now, Type = "Occupancy" };
            List<Source> sources = new List<Source>();
            sources.Add(source);
            Location location = new Location { Id = "Testeronies", Parent = "Parent", Sources = sources };
            return location;
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
