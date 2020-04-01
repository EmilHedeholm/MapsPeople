using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CoreForRabbitMQ {
    public class MappingEntry {
        public int ConsumerId { get; set; }
        public string Id { get; set; }
        public string ExternalId { get; set; }

    }
}