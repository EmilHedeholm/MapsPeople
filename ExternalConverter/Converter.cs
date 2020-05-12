﻿using DatabaseAccess;
using DataModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExternalConverter {
    public class Converter {
        //This method Converts data from the location to the external Datamodel format. 
        //Param: a Location object. 
        //Return: A list of externalmodels. 
        public List<Message> Convert(Location location, IDataAccess db) {
            List<Message> externalModels = new List<Message>();
            if (location != null) {
                if (location.Sources != null && location.Sources.Count() > 0) {
                    for (int i = 0; i < location.Sources.Count(); i++) {
                        Message externalModel = new Message();
                        externalModel.ParentIds = FindParentIds(location, db);
                        externalModel.Source = location.Sources[i];
                        externalModels.Add(externalModel);
                    }
                } else {
                    Message externalModel = new Message();
                    externalModel.ParentIds = FindParentIds(location, db);
                    externalModels.Add(externalModel);
                }
            }
            return externalModels;
        }

        //This method find all the parent IDs of a location
        //Param: a Location object.
        //Return: A string stack of the parent IDs.
        private Stack<string> FindParentIds(Location location, IDataAccess db) {
            Stack<string> parentIds = new Stack<string>();
            parentIds.Push(location.Id);
            //get the location if there is a parentId and if it's not a default ID. 
            while (location.ParentId != null && !location.ParentId.Equals("0")) {
                try {
                    location = db.GetLocationById(location.ParentId);
                }catch(Exception e) {
                    Console.WriteLine(e.Message);
                }
                parentIds.Push(location.Id);
            }
            return parentIds;
        }
    }
}
