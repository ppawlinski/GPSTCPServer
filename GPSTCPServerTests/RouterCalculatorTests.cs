using Microsoft.VisualStudio.TestTools.UnitTesting;
using GPSTCPServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPSTCPServer.Tests
{
    [TestClass()]
    public class RouterCalculatorTests
    {
        [TestMethod()]
        public void getInstructionsTest()
        {
            RouterCalculator rc = new RouterCalculator("16.9335199", "52.4082663", "16.9335199", "52.4082663");
            Assert.IsNotNull(rc.getInstructions());
            Assert.IsTrue(rc.OK);
        }
    }
}