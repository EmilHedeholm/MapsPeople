using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerAzure.JsonModel {
    public class RootObject {
        public int Id { get; set; }
        public string SpaceRefId { get; set; }
        public string Name { get; set; }
        public string SpaceType { get; set; }
        public List<LastReport> LastReports { get; set; }
    }
}
