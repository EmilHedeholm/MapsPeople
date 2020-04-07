using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace XMLSource2 {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IXMLSource {
        public List<Location> GetXMLData() {
            List<Location> data = new List<Location>();
            for (int i = 1; i < 31; i++) {
                Location location = new Location();
                location.Id = "" + i;
                Source source = new Source();
                source.Id = "" + i + 1;
                Random random = new Random();
                source.Available = random.Next() % 2 == 0 ? true : false;
                source.TimeStamp = DateTime.Now;
                location.availability = source;
                data.Add(location);
            }
            return data;
        }
    }
}
