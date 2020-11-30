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
    public class GetRequestTests
    {
        [TestMethod()]
        public void GetFromURLAsyncTest()
        {
            Task.Run(async () =>
            {
                string s = await GetRequest.GetFromURLAsync("https://nominatim.openstreetmap.org/search?q=test&format=json");
                Console.WriteLine(s);
                Assert.AreNotEqual(s, "");
            }).GetAwaiter().GetResult();
        }
    }
}