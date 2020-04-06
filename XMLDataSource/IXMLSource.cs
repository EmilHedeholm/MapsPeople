using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace XMLDataSource {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IXMLSource {
        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "GetXMLData", ResponseFormat = WebMessageFormat.Xml)]
        List<Location> GetXMLData();
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    // You can add XSD files into the project. After building the project, you can directly use the data types defined there, with the namespace "XMLSource.ContractType".
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

