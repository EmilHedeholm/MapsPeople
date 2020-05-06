using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace XMLSource2 {
    [ServiceContract]
    public interface IXMLSource {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetXMLData", ResponseFormat = WebMessageFormat.Xml)]
        List<Location> GetXMLData();
    }

    [DataContract]
    public class Location {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public Source availability { get; set; }
    }

    [DataContract]
    public class Source {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public bool Available { get; set; }
        [DataMember]
        public DateTime TimeStamp { get; set; }
    }
}

