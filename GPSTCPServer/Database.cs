using GPSTCPServer.Config;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace GPSTCPServer
{
    public class Database
    {
        //private SqlConnection connection;
        private string connectionString;
        public Database()
        {
            connectionString = ConfigHelper.CnnVal("GpsDB");
            CreateDatabaseIfNotExists(connectionString, "GPS");
            initDatabase();
        }
        public void CreateDatabaseIfNotExists(string connectionString, string dbName)
        {
            SqlCommand cmd = null;
            using (var connection = new SqlConnection(connectionString))
            {
                try{
                    connection.Open();
                }
                catch(Exception ex)
                {

                }
                
                using (cmd = new SqlCommand($"If(db_id(N'{dbName}') IS NULL) CREATE DATABASE [{dbName}]", connection))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void initDatabase()
        {
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                con.ChangeDatabase("GPS");
                using (SqlCommand command = new SqlCommand("IF OBJECT_ID(N'dbo.user', N'U') IS NULL BEGIN CREATE TABLE \"user\" (\"id\" INTEGER IDENTITY(1,1) PRIMARY KEY, \"password\" varchar(255) NOT NULL, \"username\"  varchar(20) NOT NULL UNIQUE); END;", con))
                {
                    command.ExecuteNonQuery();
                }

                using (SqlCommand command = new SqlCommand("IF OBJECT_ID(N'dbo.locations', N'U') IS NULL BEGIN CREATE TABLE \"locations\" (\"id\" INTEGER IDENTITY(1,1) PRIMARY KEY, \"name\" varchar(30) NOT NULL, \"userID\" INTEGER NOT NULL, \"osmID\" varchar(50) NOT NULL); END;", con))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
        public bool UserLogin(string username, string password)
        {
            string query = $"select password from [user] where [username]='{username}'";
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                con.ChangeDatabase("GPS");
                using (var command = new SqlCommand(query, con))
                {
                    try
                    {
                        var result = command.ExecuteReader();
                        if (result.HasRows)
                        {
                            result.Read();
                            if ((string)result["password"] == password)
                            {
                                return true;
                            }
                        }
                    }
                    catch { }
                    finally
                    {
                        command.Dispose();
                        con.Dispose();
                    }
                }

            }
            return false;
        }

        public bool CreateUser(string username, string password)
        {
            int result=-1;
            string query = $"insert into [user]([username], [password]) values ('{username}','{password}')";
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                con.ChangeDatabase("GPS");
                using (SqlCommand command = new SqlCommand(query, con))
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
                        catch { }
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

        public bool ChangePassword(string username, string password, string newpassword)
        {
            if (!UserLogin(username, password)) return false;
            int result=-1;
            string query = $"update [user] set [password]='{newpassword}' where [username]='{username}'";
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                con.ChangeDatabase("GPS");
                using (SqlCommand command = new SqlCommand(query, con))
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
                        catch (Exception)
                        {
                        }
                        finally
                        {
                            command.Dispose();
                            con.Dispose();
                        }

                    }
                    finally
                    {
                        command.Dispose();
                        con.Dispose();
                    }
                }

            }
            if (result == 1) return true;
            return false;
        }

        public bool AddLocation(string user, string name, string osmType, string osmId)
        {
            int result = -1;
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
            using(var con = new SqlConnection(connectionString))
            {
                con.Open();
                con.ChangeDatabase("GPS");
                using (SqlCommand command = new SqlCommand($"select [osmID] from [locations] where [name] = '{name}' AND [userID] IN ( select [id] from [user] where [username] = '{user}')",con))
                {
                    try
                    {
                        var res = command.ExecuteReader();
                        if(res.HasRows)
                        {
                            return false;
                        }
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        command.Dispose();
                        con.Dispose();
                    }
                }
            }

            string query = $"insert into [locations]([name],[userID],[osmID]) values ('{name}',(select [id] from [user] where [username]='{user}'),'{osm}')";
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                con.ChangeDatabase("GPS");
                using (SqlCommand command = new SqlCommand(query, con))
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
                        catch { }
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

        public List<string> GetUserLocations(string user)
        {
            string query = $"select [name] from [locations] where [userID]=(select [id] from [user] where [username]='{user}')";
            List<string> names = new List<string>();
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                con.ChangeDatabase("GPS");
                using (var command = new SqlCommand(query, con))
                {
                    var result = command.ExecuteReader();
                    try
                    {
                        if (result.HasRows)
                        {

                            while (result.Read())
                            {
                                names.Add((string)result["name"]);
                            }
                            return names;
                        }
                    }
                    catch { }
                    finally
                    {
                        command.Dispose();
                        con.Dispose();
                    }

                }

            }
            return null;
        }

        public bool DeleteLocation(string user, string location)
        {
            int result = -1;
            string query = $"delete from [locations] where [userID]=(select [id] from [user] where [username]='{user}') and [name]='{location}'";
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                con.ChangeDatabase("GPS");
                using (var command = new SqlCommand(query, con))
                {

                    try
                    {
                        result = command.ExecuteNonQuery();
                    }
                    catch(Exception)
                    {
                    }
                    finally
                    {
                        command.Dispose();
                        con.Dispose();
                    }
                }

            }
            if (result == 1) return true;
            return false;
        }

        public bool EditLocation(string user, string name, string newName, string address = null)
        {
            int result = -1;
            string query;
            if (address == null)
                query = $"update [locations] set [name]='{newName}' where [userID]=(select [id] from [user] where [username]='{user}') and [name]='{name}'";
            else
                query = $"update [locations] set [osmID]='{address}', [name]='{newName}' where [userID]=(select [id] from [user] where [username]='{user}') and [name]='{name}'";
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                con.ChangeDatabase("GPS");
                using (var command = new SqlCommand(query, con))
                {

                    try
                    {
                        result = command.ExecuteNonQuery();
                    }
                    catch (Exception)
                    {
                    }
                    finally
                    {
                        command.Dispose();
                        con.Dispose();
                    }
                }

            }
            if (result == 1) return true;
            return false;
        }

        public string GetAddress(string user, string name)
        {
            string query = $"select [osmID] from [locations] where [userID]=(select [id] from [user] where [username]='{user}' and [name]='{name}')";
            using (var con = new SqlConnection(connectionString))
            {
                con.Open();
                con.ChangeDatabase("GPS");
                using (var command = new SqlCommand(query, con))
                {
                    var result = command.ExecuteReader();
                    try
                    {
                        if (result.HasRows)
                        {
                            result.Read();
                            return (string)result["osmID"];
                        }
                    }
                    catch { }
                    finally
                    {
                        command.Dispose();
                        con.Dispose();
                    }
                }

            }
            return null;
        }
    }
}
