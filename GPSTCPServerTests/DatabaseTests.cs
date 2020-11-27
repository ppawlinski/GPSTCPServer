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
    public class DatabaseTests
    {

        [TestMethod()]
        public void CreateUserTest()
        {
            Database db = new Database("Data Source = database.sqlite3");
            byte[] data = Encoding.Default.GetBytes("test");
            string password = Encoding.UTF8.GetString(data);
            data = Encoding.Default.GetBytes("test");
            string username = Encoding.UTF8.GetString(data);
            Assert.IsFalse(db.CreateUser(username, password));
        }

        [TestMethod()]
        public void UserLoginTest()
        {
            Database db = new Database("Data Source = database.sqlite3");
            byte[] data = Encoding.Default.GetBytes("test");
            string password = Encoding.UTF8.GetString(data);
            data = Encoding.Default.GetBytes("test");
            string username = Encoding.UTF8.GetString(data);
            Assert.IsTrue(db.UserLogin(username, password));
        }

        [TestMethod()]
        public void GetAddressTest()
        {
            Database db = new Database("Data Source = database.sqlite3");
            Assert.AreEqual(db.GetAddress("test", "adrestestowy"), "W123456");
        }
    }
}