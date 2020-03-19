using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;

namespace MapspeopleConsumer.JsonModel {
    public class Geometry {
        public List<object> coordinates { get; set; }
        public List<double> bbox { get; set; }
        public string type { get; set; }

    }
}
