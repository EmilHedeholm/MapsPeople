using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerTest {
    public class ExternalModel {
        public Stack<string> ParentIds { get; set; }
        public Source Source { get; set; }
    }
}
