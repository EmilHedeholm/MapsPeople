using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModels;

namespace Send {
    class Program {
        static void Main(string[] args) {
            List<State> states = new List<State>() { new State() { Id = "Raynolds", Property = "Uppercut", Value = "Over 9000" } };
            Source sovs = new Source() { Id = "1234567", State = states, TimeStamp = DateTime.Now, Type = "Scouter" };
            List<string> parentIds = new List<string>{ "Benny", "Jhonny", "Birger" };
            ExternalHans hans = new ExternalHans() { ParentIds = parentIds, source = sovs };
            List<ExternalHans> hanser = new List<ExternalHans>();
            hanser.Add(hans);
            SendMessage.SendUpdate(hanser);

        }
    }
    public class ExternalHans {
        public List<string> ParentIds { get; set; }
        public Source source { get; set; }
    }
}