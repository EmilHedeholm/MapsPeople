using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels
{
    public class Location
    {
        public string Id { get; set; }
        public string Parent { get; set; }
        public List<Source> Sources { get; set; }

        public Location(){
         Sources = new List<Source>();
            }
    }
}
