using System;
using System.Collections.Generic;

namespace DataModels {
    public class Message {
        public Stack<string> ParentIds { get; set; }
        public Source Source { get; set; }
    }
}
