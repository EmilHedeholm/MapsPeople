using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels {
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
