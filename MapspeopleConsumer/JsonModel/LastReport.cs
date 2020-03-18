using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapspeopleConsumer.JsonModel
{
    class LastReport
    {
        public string id { get; set; }
        public int personCount { get; set; }
        public bool signsOfLife { get; set; }
        public bool motionDetected { get; set; }
        public DateTime timeStamp { get; set; }
    }
}
