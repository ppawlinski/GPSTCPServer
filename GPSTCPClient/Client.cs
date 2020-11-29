using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Windows;
using GPSTCPClient.Models;
using System.Text.Json;

namespace GPSTCPClient
{
    public static class Client
    {
        public static TcpClient TCP { get; set; }
        private static TcpListener Listener { get; set; }
        private static string login;
        public async static Task Send(string message)
        {
            await TCP.GetStream().WriteAsync(Encoding.UTF8.GetBytes(message));
        }
        public async static Task Connect(string ipAddress, int port)
        {
            TCP = new TcpClient();
            if(TCP.Connected)
            {
                return;
            }
            try
            {
                await TCP.ConnectAsync(IPAddress.Parse(ipAddress), port);
                Listener = new TcpListener(IPAddress.Parse(ipAddress), port);
            } catch(FormatException)
            {
                throw new Exception("Błędny format adresu ip");
            } catch(Exception)
            {
                throw new Exception("Błąd przy próbie połączenia");
            }
        }
        public async static Task<bool> Login(string login_, string password)
        {
            
            byte[] buffer = new byte[1024];
            string result = "";
            string md5 = Md5Hasher.CreateMD5(password);
            await Send($"LOGIN {login_} {md5}");
            result = await getUserInput(new byte[1024]);
            /*await Listener.AcceptTcpClientAsync().ContinueWith(async (listen) =>
            {
                
            });*/
            if(result == "SUCCESS")
            {
                login = login_;
                return true;
            }
            return false;
        }

        public async static Task<bool> Register(string login_, string password)
        {
            string md5 = Md5Hasher.CreateMD5(password);
            await Send($"CREATEACCOUNT {login_} {md5}");
            string result = await getUserInput(new byte[1024]);
            if (result == "ACCOUNTCREATED")
            {
                //login = login_;
                return true;
            }
            else return false;
        }
        public async static Task Logout()
        {
            login = null;
            await Send($"LOGOUT");
            //string result = "";
            //await Listener.AcceptTcpClientAsync().ContinueWith(async (listen) =>
            //{
            //    result = await getUserInput(new byte[1024]);
            //});
        }
        public static void Disconnect()
        {
            TCP.Close();
            TCP.Dispose();
            TCP = null;
            Listener = null;
        }
        private async static Task<string> getUserInput(byte[] buffer)
        {
            await TCP.GetStream().ReadAsync(buffer, 0, buffer.Length);
            //await TCP.GetStream().ReadAsync(new byte[10]);
            return Encoding.UTF8.GetString(buffer).Trim().Replace("\0", String.Empty);
        }

        public static async Task<Address[]> GetMyAddresses(string searchstring)
        {
            await Send($"GETADDRESS {searchstring.Replace(' ', '+')}");
            return JsonSerializer.Deserialize<Address[]>(await getUserInput(new byte[2048]));
        }

        public static async Task<List<UserLocation>> GetMyAddresses()
        {
            await Send($"LISTSAVEDADDRESSES");
            List<UserLocation> locations = new List<UserLocation>();
            string[] names =  (await getUserInput(new byte[1024])).Split(' ');
            try
            {
                foreach (var name in names)
                {
                    if (name == "") continue;
                    await Send($"GETSAVEDADDRESS {name}");
                    locations.Add(new UserLocation()
                    {
                        Name = name,
                        Address = JsonSerializer.Deserialize<Address[]>(await getUserInput(new byte[2048])).FirstOrDefault()
                    }
                    );
                }
            }
            catch(Exception) { return locations; }

            return locations;
        }

        public static async Task<Address[]> GetAddress(string location)
        {
            await Send($"GETADDRESS {location.Replace(' ','+')}");
            return JsonSerializer.Deserialize<Address[]>(await getUserInput(new byte[100000]));
        }

        public static async Task<string[]> GetRoute(Address origin, Address destination)
        {
            await Send($"GETROUTE {origin.Lon} {origin.Lat} {destination.Lon} {destination.Lat}");
            return (await getUserInput(new byte[100000])).Split('\n');
        }

        public static async Task<bool> AddAddress(Address address, string name)
        {
            await Send($"ADDADDRESS {name} {address.OsmType} {address.OsmId}");
            return await getUserInput(new byte[1024]) == "SUCCESS";
        }

        public static async Task<bool> DeleteAddress(string name)
        {
            await Send($"DELETEADDRESS {name}");
            return await getUserInput(new byte[1024]) == "SUCCESS";
        }
    }
}
