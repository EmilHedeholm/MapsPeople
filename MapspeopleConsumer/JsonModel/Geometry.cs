using System.Collections.Generic;

namespace MapspeopleConsumer.JsonModel {
    public class Geometry {
        public List<object> coordinates { get; set; }
        public List<double> bbox { get; set; }
        public string type { get; set; }
    }
}
