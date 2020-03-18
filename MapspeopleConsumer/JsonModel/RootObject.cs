using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapspeopleConsumer.JsonModel
{
    class RootObject
    {
        public int id { get; set; }
        public string spaceRefId { get; set; }
        public string name { get; set; }
        public string spaceType { get; set; }
        public List<LastReport> lastReports { get; set; }
    }
}
