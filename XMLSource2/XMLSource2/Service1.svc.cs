using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace XMLSource2 {
    
    public class Service1 : IXMLSource {
        //This methods makes a new list of locations data which include source data 
        // Return:A list of locations  
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
