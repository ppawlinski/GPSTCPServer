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
        private string connectionString;
        public Database(string connectionStr)
        {
            connectionString = connectionStr;
            if (!File.Exists("./database.sqlite3"))
            {
                SQLiteConnection.CreateFile("database.sqlite3");
                initDatabase();
            }
 
                
        }

        private void initDatabase()
        {
            using(var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand("CREATE TABLE \"user\" (\"id\"    INTEGER,\"password\"  TEXT NOT NULL,\"username\"  TEXT NOT NULL UNIQUE,PRIMARY KEY(\"id\" AUTOINCREMENT))", con))
                {
                    command.ExecuteNonQuery();
                }

                using (SQLiteCommand command = new SQLiteCommand("CREATE TABLE \"locations\" ( \"id\"    INTEGER, \"name\"  TEXT NOT NULL,\"userID\"    INTEGER NOT NULL,\"osmID\" TEXT NOT NULL,PRIMARY KEY(\"id\" AUTOINCREMENT))", con))
                {
                    command.ExecuteNonQuery();
                }
            } 
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
            using(var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using(var command = new SQLiteCommand(query,con))
                {
                    SQLiteDataReader result = command.ExecuteReader();
                    if (result.HasRows)
                    {
                        result.Read();
                        if ((string)result["password"] == password)
                        {
                            try { return true; }
                            finally {
                                command.Dispose();
                                con.Dispose();
                            } 
                        }
                    }
                }
  
            }
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
            using(var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, con))
                {
                    try
                    {
                        result = command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            return false;
                        }
                        finally
                        {
                            command.Dispose();
                            con.Dispose();
                        }
                        
                    }
                    if (result == 1)
                    {
                        try
                        {
                            return true;
                        }
                        finally
                        {
                            command.Dispose();
                            con.Dispose();
                        }
                        
                    }

                    try
                    {
                        return false;
                    }
                    finally
                    {
                        command.Dispose();
                        con.Dispose();
                    }
                    
                }
                
            }
            
        }

        private int getUserID(string user)
        {
            int id=-1;
            string query = $"select id from user where username=\"{user}\"";
            using(var con = new SQLiteConnection(connectionString))
            {
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, con))
                {
                    using (SQLiteDataReader result = command.ExecuteReader())
                    {
                        if (result.HasRows)
                        {
                            result.Read();
                            id = Convert.ToInt32(result["id"]);
                        }
                    }
                    
                }
                
            }
            
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
            using(var con = new SQLiteConnection(connectionString))
            {
                Console.WriteLine(".");
                con.Open();
                using (SQLiteCommand command = new SQLiteCommand(query, con))
                {
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
                        try
                        {
                            return false;
                        }
                        finally
                        {
                            command.Dispose();
                            con.Dispose();
                        }
                    }
                }
                
            }
            if (result == 1) return true;
            return false;
        }
    }
}
