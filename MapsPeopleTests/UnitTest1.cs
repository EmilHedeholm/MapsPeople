using System;
using MapspeopleConsumer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MapsPeopleTests {
    [TestClass]
    public class UnitTest1 {
        [TestMethod]
        public void TestMain() {
            //Arrange
            string rabbitMQ;

            //Act
            MapspeopleConsumer.Program.Main(null);
            rabbitMQ = Console.ReadLine();

            //Assert
            Assert.IsTrue(rabbitMQ != null);
        }
    }
}
