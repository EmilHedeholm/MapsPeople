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
                externalModel.ParentIds = FindParentIds(location);
                if (location.Sources != null && location.Sources.Count() > 0) {                                               
                    for (int i = 0; i < location.Sources.Count(); i++) {                   
                        externalModel.Source = location.Sources[i];
                        externalModels.Add(externalModel);
                    }
                } else {                          
                    externalModels.Add(externalModel);
                }
            }
            return externalModels;
        }

        //This method find all the parent IDs of a clocation
        //Param: a Location object.
        //Return: A string stack of the parent IDs.
        private Stack<string> FindParentIds(Location location) {
            IDataAccess dataAccess = new SQLDataAccess();
            Stack<string> parentIds = new Stack<string>();
            parentIds.Push(location.Id);
            
            while (!location.ParentId.Equals("0")) {           
                location= dataAccess.GetLocationById(location.ParentId);
                parentIds.Push(location.Id);
            }
            return parentIds;
        }

    }
}