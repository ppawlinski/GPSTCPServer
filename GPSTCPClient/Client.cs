using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using GPSTCPClient.Models;
using System.Text.Json;
using System.IO;
using GPSTCPClient.ViewModel.Components;
using System.Globalization;

namespace GPSTCPClient
{
    public static class Client
    {
        public static TcpClient TCP { get; set; }
        private static TcpListener listener { get; set; }
        private static string login;
        public static string ApiKey;
        public async static void Send(string message)
        {
            await TCP.GetStream().WriteAsync(Encoding.UTF8.GetBytes(message));
        }
        public async static Task<bool> Connect(string ipAddress, int port)
        {
            TCP = new TcpClient();
            if (TCP.Connected)
            {
                return true;
            }
            try
            {
                await TCP.ConnectAsync(IPAddress.Parse(ipAddress), port);
                listener = new TcpListener(IPAddress.Parse(ipAddress), port);
                return true;
            }
            catch (FormatException)
            {
                throw new Exception("Błędny format adresu ip");
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async static Task<bool> Login(string login_, string password)
        {

            byte[] buffer = new byte[1024];
            string result = "";
            string md5 = Md5Hasher.CreateMD5(password);
            Send($"LOGIN {login_} {md5}");
            result = await getUserInput(new byte[1024]);
            /*await Listener.AcceptTcpClientAsync().ContinueWith(async (listen) =>
            {
                
            });*/
            if (result == "SUCCESS")
            {
                login = login_;
                return true;
            }
            return false;
        }

        public async static Task<bool> Register(string login_, string password)
        {
            string md5 = Md5Hasher.CreateMD5(password);
            Send($"CREATEACCOUNT {login_} {md5}");
            string result = await getUserInput(new byte[1024]);
            if (result == "ACCOUNTCREATED")
            {
                //login = login_;
                return true;
            }
            else return false;
        }
        public static Task Logout()
        {
            login = null;
            return Task.Run(() => Send($"LOGOUT"));
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
            listener = null;
        }
        private async static Task<string> getUserInput(byte[] buffer)
        {
            await TCP.GetStream().ReadAsync(buffer, 0, buffer.Length);
            //await TCP.GetStream().ReadAsync(new byte[10]);
            return Encoding.UTF8.GetString(buffer).Trim().Replace("\0", String.Empty);
        }

        public static async Task<List<UserLocation>> GetMyAddresses()
        {
            Send($"LISTSAVEDADDRESSES");
            List<UserLocation> locations = new List<UserLocation>();
            string[] names = (await getUserInput(new byte[1024])).Split(' ');
            if (names[0] != "FAIL")
            {
                try
                {
                    foreach (var name in names)
                    {
                        if (name == "") continue;
                        Send($"GETSAVEDADDRESS {name}");
                        var resp = await getUserInput(new byte[2048]);
                        var parsed = JsonSerializer.Deserialize<Address[]>(resp);
                        locations.Add(new UserLocation()
                        {
                            Name = name,
                            Address = parsed.FirstOrDefault()
                        }
                        );
                    }
                }
                catch (Exception ex) {
                    var c = ex.Message;
                    return locations; 
                }
            }
            return locations;
        }

        public static async Task<Address[]> GetAddress(string location)
        {
            try
            {
                Send($"GETADDRESS {location.Replace(' ', '+')}");
                return JsonSerializer.Deserialize<Address[]>(await getUserInput(new byte[100000]));
            }
            catch (Exception)
            {
                return new Address[0];
            }
            
        }

        public static async Task<RouteModel> GetRoute(Address origin, Address destination)
        {
            Send($"GETROUTE {origin.Lon.ToString(CultureInfo.InvariantCulture)} {origin.Lat.ToString(CultureInfo.InvariantCulture)} {destination.Lon.ToString(CultureInfo.InvariantCulture)} {destination.Lat.ToString(CultureInfo.InvariantCulture)}");
            var result = await getUserInput(new byte[1000000]);
            if (result == "FAIL") return null;
            return JsonSerializer.Deserialize<RouteModel>(result);
        }

        public static async Task<bool> AddAddress(Address address, string name)
        {
            Send($"ADDADDRESS {name} {address.OsmType} {address.OsmId}");
            return await getUserInput(new byte[1024]) == "SUCCESS";
        }

        public static async Task<bool> DeleteAddress(string name)
        {
            Send($"DELETEADDRESS {name}");
            return await getUserInput(new byte[1024]) == "SUCCESS";
        }

        public static async Task<bool> ChangePassword(string oldPassword, string newPassword)
        {
            oldPassword = Md5Hasher.CreateMD5(oldPassword);
            newPassword = Md5Hasher.CreateMD5(newPassword);
            Send($"CHANGEPASSWORD {oldPassword} {newPassword}");
            return await getUserInput(new byte[1024]) == "SUCCESS";
        }

        public static async Task<Address> DescribeAddress(double lat, double lon)
        {
            Send($"DESCRIBEADDRESS {lat.ToString(CultureInfo.InvariantCulture)} {lon.ToString(CultureInfo.InvariantCulture)}");
            return JsonSerializer.Deserialize<Address>(await getUserInput(new byte[10000]));
        }

        public static async Task<bool> EditAddress(string name,string newName, Address address)
        {
            Send($"EDITADDRESS {name.Replace(' ','+')} {newName.Replace(' ','+')} {address.OsmType} {address.OsmId}");
            return await getUserInput(new byte[1024]) == "SUCCESS";
        }
    }
}
