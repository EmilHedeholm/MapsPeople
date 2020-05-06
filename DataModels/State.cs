using System;

namespace DataModels {
    //This class represents state the the internal datamodel. 
    public class State : IEquatable<State> {
        public string Property { get; set; }
        public string Value { get; set; }

        public bool Equals(State other) {
            if (this.Property.Equals(other.Property)) {
                return true;
            } else {
                return false;
            }
        }
    }
}
