using System;
using System.Collections.Generic;

namespace DataModels {
    public class Source : IEquatable<Source> {
        
        public string Type { get; set; }
        public List<State> State { get; set; }
        public DateTime TimeStamp { get; set; }

        public Source(){
            State = new List<State>();
            Type = "0";
            TimeStamp = DateTime.Now;
        }

        public bool Equals(Source other) {
            if (this.Type.Equals(other.Type)) {
                return true;
            } else {
                return false;
            }
        }

        public override int GetHashCode() {
            return this.Type.GetHashCode();
        }
    }
}
