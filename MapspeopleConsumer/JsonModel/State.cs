using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapspeopleConsumer.JsonModel
{
    class State
    {
        public int PersonCount { get; set; }
        public bool SignsOfLife { get; set; }
        public bool MotionDetected { get; set; }
    }
}
