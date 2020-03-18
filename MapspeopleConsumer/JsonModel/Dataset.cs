using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapspeopleConsumer.JsonModel {
    class Dataset {

        public string Id { get; set; }
        public string  Name { get; set; }
        public List<string> AvailableLanguages { get; set; }
        public string BaseLanguage { get; set; }
        public List<string> RootObjects { get; set; }

    }
}
