using System;

namespace ConsumerAzure.JsonModel {
    public class LastReport {
        public string Id { get; set; }
        public int PersonCount { get; set; }
        public bool SignsOfLife { get; set; }
        public bool MotionDetected { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
