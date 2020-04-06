using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace XMLConsumer.Models {
    [XmlRoot(ElementName = "availability", Namespace = "http://schemas.datacontract.org/2004/07/XMLSource2")]
    public class Availability {
        [XmlElement(ElementName = "Available", Namespace = "http://schemas.datacontract.org/2004/07/XMLSource2")]
        public string Available { get; set; }
        [XmlElement(ElementName = "Id", Namespace = "http://schemas.datacontract.org/2004/07/XMLSource2")]
        public string Id { get; set; }
        [XmlElement(ElementName = "TimeStamp", Namespace = "http://schemas.datacontract.org/2004/07/XMLSource2")]
        public string TimeStamp { get; set; }
    }
}
