using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseAccess;
using DataModels;

namespace Core.Service {
    public class ExternalConverter {

        //This method Converts data from the location to the external Datamodel format. 
        //Param: a Location object. 
        //Return: A list of externalmodels. 
        public List<ExternalModel> Convert(Location location) {
            List<ExternalModel> externalModels = new List<ExternalModel>();
            ExternalModel externalModel = new ExternalModel();
            if (location != null) {
                externalModel.LocationId = location.Id;
                
                if (location.Sources != null && location.Sources.Count() > 0) {
                    if (!location.ParentId.Equals("0")) {
                        externalModel.ParentIds = FindParentIds(location);
                    }
                    for (int i = 0; i < location.Sources.Count(); i++) {                   
                        externalModel.Source = location.Sources[i];
                        externalModels.Add(externalModel);
                    }
                } else {
                    if (!location.ParentId.Equals("0")) {
                        externalModel.ParentIds = FindParentIds(location);
                    }
                    externalModels.Add(externalModel);
                }
            }
            return externalModels;
        }

        //This method find all the parent IDs of a clocation
        //Param: a Location object.
        //Return: A string list of the parent IDs.
        private List<string> FindParentIds(Location location) {
            IDataAccess dataAccess = new DataAccess();
            List<string> parentIds = new List<string>();
            parentIds.Add(location.ParentId);
            Location l1 = dataAccess.GetLocationById(location.ParentId);
            if (!l1.ParentId.Equals("0")) {
                parentIds.Add(l1.ParentId);
                Location l2 = dataAccess.GetLocationById(l1.ParentId);
                if (!l2.ParentId.Equals("0")) {
                    parentIds.Add(l2.ParentId);
                }
            }
            return parentIds;
        }

    }
}