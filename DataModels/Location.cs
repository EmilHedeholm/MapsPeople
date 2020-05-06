using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace DataModels {
    //This class represents Location from the internal datamodel. 
    public class Location : IEquatable<Location> {
        [BsonId]
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string ExternalId { get; set; }
        public int ConsumerId { get; set; }
        public List<Source> Sources { get; set; }

        //This constructor creates default ID's on a location.
        public Location(){
            Sources = new List<Source>();
            Id = "0";
            ParentId = "0";
            ExternalId = "0";
            ConsumerId = 0;
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
        public bool Equals(Location other) {
            if (this.Id.Equals(other.Id)) {
                return true;
            } else {
                return false;
            }
        }
    }
}
