using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Controllers;
using DataModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MapsPeopleTests {
    class ReceivingControllerTest {
        ReceivingController receivingController;
        [TestMethod]
        public void TestPostMethod() {
            //Arrange
            receivingController = new ReceivingController();
            IEnumerable<Location> locations = new List<Location>();


        }
   }
}
