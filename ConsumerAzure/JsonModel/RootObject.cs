using System.Collections.Generic;

namespace ConsumerAzure.JsonModel {
    public class RootObject {
        public int Id { get; set; }
        public string SpaceRefId { get; set; }
        public string Name { get; set; }
        public string SpaceType { get; set; }
        public List<LastReport> LastReports { get; set; }
    }
}
