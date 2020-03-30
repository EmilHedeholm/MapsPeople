using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseAccess;
using DataModels;

namespace Core.Service {
   
    public class ExternalConverter {
        IDataAccess dataAccess = new DataAccess();
        public List<ExternalModel> ConvertToExternalModel (Location location) {
            List<ExternalModel> externalModels = new List<ExternalModel>();
            ExternalModel externalModel = new ExternalModel();
            if(location != null) { 
                 if (location.Sources != null && location.Sources.Count() > 0) {
                    for (int i = 0; i < location.Sources.Count(); i++) {
                        externalModel = ConvertFromInternalToExternalModel(location, i);
                        externalModels.Add(externalModel);

                     } 
                 } else {
                    externalModel.LocationId = location.Id;
                    if (location.ParentId != null) {
                        externalModel.ParentIds = FindParentIds(location);
                    }
                    externalModels.Add(externalModel);
                }
            }
            return externalModels;

    }

        private List<string> FindParentIds(Location location) {
            List<string> parentIds = new List<string>();
            Location l1 = dataAccess.GetLocationById(location.Id);
            parentIds.Add(l1.ParentId);
            if (l1.ParentId != null) {
                Location l2 = dataAccess.GetLocationById(l1.ParentId);
                parentIds.Add(l2.ParentId);
                if(l2.ParentId != null) {
                    Location l3 = dataAccess.GetLocationById(l2.ParentId);
                    parentIds.Add(l3.ParentId);
                }

            }

            return parentIds;
        }

        private ExternalModel ConvertFromInternalToExternalModel(Location location, int i) {
            
        }
    }