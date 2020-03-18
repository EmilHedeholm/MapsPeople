﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapspeopleConsumer.JsonModel {
    class BaseTypeProperties {
        public string defaultfloor { get; set; }
        public string administrativeid { get; set; }
        public string graphid { get; set; }
        public string name { get; set; }
        public string @class { get; set; }
        public string imageurl { get; set; }
        public DateTime? activefrom { get; set; }

    }
}
