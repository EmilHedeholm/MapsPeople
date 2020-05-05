using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;

namespace ClientGUI {
    public class Message :IEquatable<Message>{
        public string LocationId { get; set;}
        public List<Source> Sources { get; set; }

        public Message() {
            Sources = new List<Source>();
        }

        public bool Equals(Message other) {
            if (this.LocationId.Equals(other.LocationId)) {
                return true;
            } else return false;
        }

        public override int GetHashCode() {
            return LocationId.GetHashCode();
        }
    }
}
