using System;
using System.Collections.Generic;

namespace DataModels {
    //This class represents the external datamodel. 
    public class Message {
        public Stack<string> ParentIds { get; set; }
        public Source Source { get; set; }
    }
}
