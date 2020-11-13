using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GPSTCPServer
{
    public class Database
    {
        //private SQLiteConnection connection;
        public SQLiteConnection connection;
        public Database()
        {
            connection = new SQLiteConnection("Data Source=database.sqlite3");

            if (!File.Exists("./database.sqlite3"))
                SQLiteConnection.CreateFile("database.sqlite3");
        }

        public void OpenConnection()
        {
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();
        }

        public void CloseConnection()
        {
            if (connection.State != System.Data.ConnectionState.Closed)
                connection.Close();
        }
        public bool UserLogin(string username, string password)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(password));
                password = Convert.ToBase64String(data);
            }

            string query = $"select password from user where username=\"{username}\"";
            SQLiteCommand command = new SQLiteCommand(query, this.connection);
            OpenConnection();
            SQLiteDataReader result = command.ExecuteReader();
            if (result.HasRows)
            {
                result.Read();
                if ((string)result["password"] == password)
                {
                    CloseConnection();
                    return true;
                }
            }
            CloseConnection();
            return false;
        }

        public bool CreateUser(string username, string password)
        {
            int result;
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] data = md5.ComputeHash(utf8.GetBytes(password));
                password = Convert.ToBase64String(data);
            }
            string query = $"insert into user(username, password) values (\"{username}\",\"{password}\")";
            SQLiteCommand command = new SQLiteCommand(query, this.connection);
            OpenConnection();
            try
            {
                result = command.ExecuteNonQuery();
            }
            catch(Exception)
            {
                CloseConnection();
                return false;
            }
            CloseConnection();
            if (result == 1) return true;
            return false;
        }

        private int getUserID(string user)
        {
            int id=-1;
            string query = $"select id from user where username=\"{user}\"";
            SQLiteCommand command = new SQLiteCommand(query, this.connection);
            OpenConnection();
            SQLiteDataReader result = command.ExecuteReader();

            if (result.HasRows)
            {
                result.Read();
                id = Convert.ToInt32(result["id"]);
            }
            CloseConnection();
            return id;
        }

        public bool AddLocation(string user, string osmType ,long osmId, string name)
        {
            int result;
            //to zapytanie jeszcze działa
            //int userID = getUserID(user);
            string osm = "";
            switch (osmType)
            {
                case "node":
                    osm = "N" + osmId;
                    break;
                case "way":
                    osm = "W" + osmId;
                    break;
                case "relation":
                    osm = "R" + osmId;
                    break;
            }
            string query = $"insert into locations(name,userID,osmID) values (\"{name}\",(select id from user where username=\"{user}\"),\"{osm}\")";
            SQLiteCommand command = new SQLiteCommand(query, this.connection);
            OpenConnection();
            Console.WriteLine(command.CommandText);
            //w tym momencie się blokuje baza
            try
            {
                result = command.ExecuteNonQuery();
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConnection();
                return false;
            }
            CloseConnection();
            if (result == 1) return true;
            return false;
        }
    }
}
