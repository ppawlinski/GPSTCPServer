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
            db.FillWithTestData();
            string password = Encoding.UTF8.GetString(Encoding.Default.GetBytes("test"));
            string username = "test";
            Assert.IsFalse(db.CreateUser(username, password));
            Assert.IsTrue(db.CreateUser("test2", "test2"));
        }

        [TestMethod()]
        public void UserLoginTest()
        {
            Database db = new Database("Data Source = database.sqlite3");
            db.FillWithTestData();
            byte[] data = Encoding.Default.GetBytes("test");
            string password = Encoding.UTF8.GetString(data);
            data = Encoding.Default.GetBytes("test");
            string username = Encoding.UTF8.GetString(data);
            Assert.IsTrue(db.UserLogin(username, password));
            Assert.IsFalse(db.UserLogin("", ""));
        }

        [TestMethod()]
        public void GetAddressTest()
        {
            Database db = new Database("Data Source = database.sqlite3");
            db.FillWithTestData();
            Assert.AreEqual("R2989158", db.GetAddress("test", "Poznan"));
            Assert.AreEqual(null, db.GetAddress("test", "nieistniejacy"));
        }

        [TestMethod()]
        public void AddLocationTest()
        {
            Database db = new Database("Data Source = database.sqlite3");
            db.FillWithTestData();
            Assert.IsTrue(db.AddLocation("test", "dom", "node", "1234567"));
            Assert.AreEqual(db.GetAddress("test", "dom"), "N1234567");
        }

        [TestMethod()]
        public void getUserLocationsTest()
        {
            Database db = new Database("Data Source = database.sqlite3");
            db.FillWithTestData();
            Assert.IsNotNull(db.GetUserLocations("test"));
            Assert.AreEqual(db.GetUserLocations("test2"), null);
        }

        [TestMethod()]
        public void DeleteLocationTest()
        {
            Database db = new Database("Data Source = database.sqlite3");
            db.FillWithTestData();
            Assert.IsTrue(db.DeleteLocation("test", "Poznan"));
            Assert.AreEqual(db.GetUserLocations("test"), null);
        }

        [TestMethod()]
        public void EditLocationTest()
        {
            Database db = new Database("Data Source = database.sqlite3");
            db.FillWithTestData();
            Assert.IsTrue(db.EditLocation("test", "Poznan", "ZmienionyPoznan", "R7654321"));
            Assert.AreEqual(db.GetAddress("test", "ZmienionyPoznan"), "R7654321");
        }
    }
}