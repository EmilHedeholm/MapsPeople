using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModels {
    public class MappingEntry {
        public string Id { get; set; }
        public int ConsumerId { get; set; }
        public string ExternalId { get; set; }
    }
}
