using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsumerTest {
    class Program {
        static void Main(string[] args) {
            Receiver receiver = new Receiver();
            receiver.Consume();
        }
    }
}
