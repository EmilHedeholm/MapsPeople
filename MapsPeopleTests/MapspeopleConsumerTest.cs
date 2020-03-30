using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MapspeopleConsumer;
using RestSharp;

namespace MapsPeopleTests {
    [TestClass]
    public class MapspeopleConsumerTest {
        Program mapsPeopleConsumer;
        [TestMethod]
        public void TestGetTokenMethod() {
            //Arrange
            mapsPeopleConsumer = new Program();
            var client = new RestClient();

            mapsPeopleConsumer.GetToken();

        }
    }
}
