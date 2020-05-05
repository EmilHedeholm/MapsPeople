using System.Xml.Serialization;
using System.Collections.Generic;
namespace XMLConsumer.Models {
    [XmlRoot(ElementName = "ArrayOfLocation", Namespace = "http://schemas.datacontract.org/2004/07/XMLSource2")]
    public class ArrayOfLocation {
        [XmlElement(ElementName = "Location", Namespace = "http://schemas.datacontract.org/2004/07/XMLSource2")]
        public List<Location> Location { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "i", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string I { get; set; }

        public ArrayOfLocation() {
            Location = new List<Location>();
        }
    }
}
