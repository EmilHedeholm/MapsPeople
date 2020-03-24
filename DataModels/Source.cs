using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels {
    public class Source {
        public string Id { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> State { get; set; }
        public DateTime TimeStamp { get; set; }

        public Source(){
            State = new Dictionary<string, string>();
            Id = "0";
            Type = "0";
            TimeStamp = DateTime.Now;
        }
    }
}
