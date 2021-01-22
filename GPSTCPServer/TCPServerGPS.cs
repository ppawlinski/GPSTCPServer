using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using GPSTCPServer.Models;

namespace GPSTCPServer
{
    public class TCPServerGPS : TCPServerCore
    {
        private Database db;
        public TCPServerGPS(IPAddress iP, int port) : base(iP, port)
        {
            this.db = new Database();
        }

        public async Task RunServer()
        {
            Listener.Start();
            while (true)
            {
                _ = await Listener.AcceptTcpClientAsync().ContinueWith(async (t) =>
                  {
                      byte[] buffer = new byte[1024];
                      GPSUser user = new GPSUser(t.Result);
                      while (true)
                      {
                          string input = await getUserInput(user.client, buffer);
                          Array.Clear(buffer, 0, buffer.Length);
                          Console.WriteLine($"CLIENT: {input}");
                          _ = Task.Run(async () =>
                            {
                                string response = "";
                                try
                                {
                                    response = await processCommand(user, input);
                                }
                                catch(Exception ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                                
                              Console.WriteLine($"SERVER: {response}");
                              await Send(user.client, response);
                            });
                      }

                  });
            }
        }

        private async Task<string> processCommand(GPSUser user,string fullMessage)
        {
            
            string[] arguments = fullMessage.Split(" ");
            string command = arguments[0];
            if (command == "LOGIN")
            {
                if (user.LoggedIn || arguments.Length != 3)
                {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
                string arg1 = arguments[1].Trim();
                string arg2 = arguments[2].Trim();
                string username = await login(arg1, arg2);
                if (username != null)
                {
                    //login succesful
                    user.Username = username;
                    user.LoggedIn = true;
                    return "SUCCESS";
                }
                else
                {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
            }
            else if (command == "LOGOUT")
            {
                if (!user.LoggedIn)
                {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
                user.LoggedIn = false;
                user.Username = null;
                return "SUCCESS";
            }
            else if (command == "CREATEACCOUNT")
            {
                if (user.LoggedIn || arguments.Length != 3)
                {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
                if (await createAccount(arguments[1], arguments[2]))
                {
                    return "ACCOUNTCREATED";
                }
                else
                {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
            }
            else if (command == "CHANGEPASSWORD")
            {
                if (!user.LoggedIn || arguments.Length != 3) return "FAIL";
                if (db.ChangePassword(user.Username, arguments[1], arguments[2]))
                {
                    return "SUCCESS";
                }
                else
                {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
            }
            else if (command == "GETADDRESS")
            {
                if (!user.LoggedIn) return "FAIL";
                //assuming that client replaced spaces with +
                Address[] address = await getAddresses(arguments[1]);
                if (address != null)
                {
                    return JsonSerializer.Serialize<Address[]>(address);
                }
                else
                {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
            }
            else if (command == "GETROUTE")
            {
                if (!user.LoggedIn) return "FAIL";
                RouteModel instructions = await getRoute(arguments[1], arguments[2], arguments[3], arguments[4]);
                if (instructions != null) return JsonSerializer.Serialize(instructions);
                else
                {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
            }
            else if (command == "LISTSAVEDADDRESSES")
            {
                if (!user.LoggedIn) return "FAIL";
                string result = await listSavedAddressess(user.Username);
                if (result != null) return result;
                else
                {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
            }
            else if (command == "GETSAVEDADDRESS")
            {
                if (!user.LoggedIn) return "FAIL";
                return await getSavedAddress(user, arguments[1]);
            }
            else if (command == "EDITADDRESS")
            {
                if (!user.LoggedIn) {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
                if (arguments.Length == 3)
                {
                    if (await editAddress(user.Username, arguments[1], arguments[2]))
                    {
                        return "SUCCESS";
                    }
                }
                else if (arguments.Length == 5)
                {
                    if (await editAddress(user.Username, arguments[1], arguments[2], arguments[3], arguments[4]))
                    {
                        return "SUCCESS";
                    }
                }
                else
                {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
            }
            else if (command == "ADDADDRESS")
            {
                if (!user.LoggedIn || arguments.Length != 4)
                {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
                if (db.AddLocation(user.Username, arguments[1], arguments[2], arguments[3])) return "SUCCESS";
                else {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
            }
            else if (command == "DELETEADDRESS")
            {
                if (!user.LoggedIn || arguments.Length != 2) {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
                if (db.DeleteLocation(user.Username, arguments[1])) return "SUCCESS";
                else {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
            }
            else if (command == "DESCRIBEADDRESS")
            {
                if (!user.LoggedIn || arguments.Length != 3)
                {
#if DEBUG
                    Console.WriteLine("PAKIET: " + fullMessage);
#endif
                    return "FAIL";
                }
                return await getAddressInfo(arguments[1], arguments[2]);
            }
            return "UNKNOWNCOMMAND";
        }

        private Task<bool> editAddress(String username, string name, string newName, string osmType = null, string osmId = null)
        {
            if (osmId == null || osmType == null)
            {
                return Task.FromResult(db.EditLocation(username, name, newName));
            }
            string osm = string.Empty;
            //should be done on clients side
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
            return Task.FromResult(db.EditLocation(username, name, newName, osm));
        }

        private Task<string> listSavedAddressess(string username)
        {
            List<string> names = db.GetUserLocations(username);
            if (names == null) return Task.FromResult<string>(null);
            string result = string.Empty;
            if (names.Count == 0) return Task.FromResult<string>(null);
            foreach (var name in names)
            {
                result += name + " ";
            }
            return Task.FromResult(result);
        }

        private async Task<string> getSavedAddress(GPSUser user, string name)
        {
            var names = db.GetUserLocations(user.Username);
            if (names != null)
            {
                int i = 0;
                foreach (var n in names)
                {
                    if (names[i] == name) break;
                    i++;
                }

                string osmID = db.GetAddress(user.Username, names[i]);
                string address = await GetRequest.GetFromURLAsync(String.Format("https://nominatim.openstreetmap.org/lookup?osm_ids={0}&format=json", osmID));
                return address;

            }
            else
                return null;
        }

        private Task<RouteModel> getRoute(string originLon, string originLat, string destinationLon, string destinationLat)
        {
            string response = string.Empty;
            RouterCalculator calculator = new RouterCalculator(originLon, originLat, destinationLon, destinationLat);
            if (!calculator.OK)
            {
                return Task.FromResult<RouteModel>(null);
            }
            RouteModel instructions = calculator.GetInstructions();
            //foreach (var instruction in instructions)
            //{
            //    if (instruction != string.Empty) response += instruction + "\n";
            //}
            return Task.FromResult(instructions);
        }

        private async Task<Address> getAddress(string message)
        {
            Address[] address = JsonSerializer.Deserialize<Address[]>(await GetRequest.GetFromURLAsync(String.Format("https://nominatim.openstreetmap.org/search?q={0}&format=json", message.Replace(" ", "+"))));
            if (address.Length == 0)
            {
                //address not found
                return null;
            }
            else if (address.Length > 0)
            {
                //return address
                return address[0];
            }
            else throw new Exception();
        }

        private async Task<Address[]> getAddresses(string message)
        {
            Address[] address = JsonSerializer.Deserialize<Address[]>(await GetRequest.GetFromURLAsync(String.Format("https://nominatim.openstreetmap.org/search?q={0}&format=json", message.Replace(" ", "+"))));
            if (address.Length == 0)
            {
                //address not found
                return null;
            }
            else if (address.Length > 0)
            {
                //return address
                return address;
            }
            else throw new Exception();
        }

        private async Task<string> getAddressInfo(string lat, string lon)
        {
            var response = await GetRequest.GetFromURLAsync(String.Format("https://nominatim.openstreetmap.org/reverse?lat={0}&lon={1}&format=json", lat, lon));
            return response;
        }

        private Task<string> login(string username, string password)
        {

            if (db.UserLogin(username, password))
            {
                return Task.FromResult(username);
            }

            return Task.FromResult<string>(null);
        }

        private async Task<bool> createAccount(string username, string password)
        {
            if (db.CreateUser(username, password))
            {
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        private async Task<string> getUserInput(TcpClient client, byte[] buffer)
        {
            Array.Clear(buffer, 0, buffer.Length);
            await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
            //await client.GetStream().ReadAsync(new byte[10]);
            return Encoding.UTF8.GetString(buffer).Trim().Replace("\0", String.Empty);
        }

        public static async Task Send(TcpClient client, string message)
        {
            await client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(message));
        }

        public static async Task SendLine(TcpClient client, string message)
        {
            await client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(message + Environment.NewLine));
        }
    }
}
