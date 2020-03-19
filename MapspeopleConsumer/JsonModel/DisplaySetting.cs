﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapspeopleConsumer.JsonModel {
    class DisplaySetting {
        public string name { get; set; }
        public string iconUrl { get; set; }
        public double? zoomFrom { get; set; }
        public double? iconScale { get; set; }
        public IconSize iconSize { get; set; }
        public string labelTemplate { get; set; }

    }
}