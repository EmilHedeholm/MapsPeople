using System;
using System.Collections.Generic;

namespace Client.Models {
    public class Source {
        public string Id { get; set; }
        public string Type { get; set; }
        public List<State> State { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}