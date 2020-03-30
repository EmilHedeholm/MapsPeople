using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Client.Models {
    public class Message {
        public List<string> ParentIds { get; set; }
        public Source source { get; set; }
    }
}