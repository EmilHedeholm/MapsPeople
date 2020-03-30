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
        public string ParentId { get; set; }
        public string ExternalId { get; set; }
        public int ConsumerId { get; set; }
        public List<Source> Sources { get; set; }

        public Location(){
            Sources = new List<Source>();
            Id = "0";
            ParentId = "0";
            ExternalId = "0";
            ConsumerId = 0;
        }
    }
}
