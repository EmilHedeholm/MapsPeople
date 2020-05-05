﻿using System.Xml.Serialization;

namespace XMLConsumer.Models {
    [XmlRoot(ElementName = "Location", Namespace = "http://schemas.datacontract.org/2004/07/XMLSource2")]
    public class Location {
        [XmlElement(ElementName = "Id", Namespace = "http://schemas.datacontract.org/2004/07/XMLSource2")]
        public string Id { get; set; }
        [XmlElement(ElementName = "availability", Namespace = "http://schemas.datacontract.org/2004/07/XMLSource2")]
        public Availability Availability { get; set; }

        public Location() {
            Availability = new Availability();
        }
    }
}
