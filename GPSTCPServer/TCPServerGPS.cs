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
        public TCPServerGPS(IPAddress iP, int port, Database db) : base(iP, port)
        {
            this.db = db;
        }

        public async Task RunServer()
        {
            Listener.Start();
            while (true)
            {
                await Listener.AcceptTcpClientAsync().ContinueWith(async (t) =>
                {
                    byte[] buffer = new byte[1024];
                    GPSUser user = new GPSUser(t.Result);
                    while (true)
                    {
                        await getUserInput(user.client, buffer);
                        string response = await ProcessCommand(user, buffer);
                        await Send(user.client, response);
                    }

                });
            }
        }

        private async Task<string> ProcessCommand(GPSUser user, byte[] buffer)
        {
            string fullMessage = Encoding.UTF8.GetString(buffer).Replace("\0", String.Empty);
            string[] arguments = fullMessage.Split(",");
            string command = arguments[0];
            if (command == "LOGIN")
            {
                if (user.LoggedIn || arguments.Length<3)    return "LOGINFAIL";
                string arg1 = arguments[1].Trim();
                string arg2 = arguments[2].Trim();
                string username = await Login(arg1, arg2);
                if (username != null)
                {
                    //login succesful
                    user.Username = username;
                    user.LoggedIn = true;
                    return "LOGINSUCCESS";
                }
                else
                {
                    return "LOGINFAIL";
                }
            }
            else if (command == "CREATEACCOUNT")
            {
                if (user.LoggedIn) throw new Exception();
                if (await createAccount(arguments[1], arguments[2]))
                {
                    return "ACCOUNTCREATED";
                }
                else
                {
                    return "ACCOUNTEXISTS";
                }
            }
            else if (command == "GETADDRESS")
            {
                if (!user.LoggedIn) throw new Exception();
                //assuming that client replaced spaces with +
                Address[] address = await getAddresses(arguments[1]);
                if (address != null)
                {
                    //send address/list
                    string result = JsonSerializer.Serialize<Address[]>(address);
                    return "ADDRESSES " + result;
                }
                else
                {
                    return "ADDRESSNOTFOUND";
                }
            }
            else if (command == "GETROUTE")
            {
                if (!user.LoggedIn) throw new Exception();
                Address addr1 = await getAddress(arguments[1]);
                Address addr2 = await getAddress(arguments[2]);
                string instructions = await getRoute(addr1, addr2);
                return "INSTRUCTIONS " + instructions;
            }
            else if (command == "LISTSAVEDADDRESSES")
            {
                if (!user.LoggedIn) throw new Exception();
                string result = await ListSavedAddressess(user.Username);
                return "SAVEDLIST " + result;
            }
            else if (command == "GETSAVEDADDRESS")
            {
                if (!user.LoggedIn) throw new Exception();
                string result = await getSavedAddress(user, arguments[1]);
                return "SAVEDADDRESS " + result;

            }
            return "UNKNOWNCOMMAND";
        }

        private async Task<string> ListSavedAddressess(string username)
        {
            List<string> names = db.getUserLocations(username);
            string result = string.Empty;
            foreach(var name in names)
            {
                result += name + ",";
            }
            return result;
        }

        private async Task<string> getSavedAddress(GPSUser user, string name)
        {
            var names = db.getUserLocations(user.Username);
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

        private async Task<string> getRoute(Address origin, Address destination)
        {
            string response = string.Empty;
            RouterCalculator calculator = new RouterCalculator(origin, destination);
            if (!calculator.OK)
            {
                response = "Nie można znaleźć połączenia między podanymi punktami";
            }
            string[] instructions = calculator.getInstructions();
            foreach (var instruction in instructions)
            {
                if (instruction != string.Empty) response += instruction + "\n";
            }
            return response;
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

        private async Task<string> Login(string username, string password)
        {

            if (db.UserLogin(username, password))
            {
                return username;
            }

            return null;
        }
        private async Task<bool> createAccount(string username, string password)
        {
            if (db.CreateUser(username, password))
            {
                return true;
            }
            return false;
        }
        private async Task<string> getUserInput(TcpClient client, byte[] buffer)
        {
            Array.Clear(buffer, 0, buffer.Length);
            await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
            await client.GetStream().ReadAsync(new byte[10]);
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


        //client side
        /*private async Task<bool> manageSavedLocations(TcpClient client, byte[] buffer, string user)
        {
            //add location
            if (choice == 0)
            {
                Address addr = null;
                string message;
                while (addr == null)
                {
                    await Send(client, "Podaj adres: ");
                    message = await getUserInput(client, buffer);
                    addr = await getAddress(client, buffer, message);
                }

                await Send(client, "Podaj nazwę pod jaką chcesz zapisać lokalizację: ");
                string name = await getUserInput(client, buffer);
                if (!db.AddLocation(user, addr.OsmType, addr.OsmId, name))
                {
                    await SendLine(client, "Nie udało się zapisać lokalizacji w bazie!");
                    return false;
                }
                await SendLine(client, "Pomyślnie zapisano lokalizację.");
                return true;
            }
            //managing existing locations
            else if (choice == 1 || choice == 2)
            {
                var names = db.getUserLocations(user);
                if (names != null)
                {
                    //listing locations
                    await SendLine(client, "Wybierz lokalizację:");
                    int i = 0;
                    foreach (var n in names)
                    {
                        await SendLine(client, $"[{i}] {n}");
                        i++;
                    }
                    await SendLine(client, $"[{names.Count}] Anuluj");
                    int index = -1;
                    while (index < 0 || index >= names.Count)
                    {
                        if (index == names.Count) return false;
                        index = await getUserInputInt(client, buffer);
                    }

                    //editing location
                    if (choice == 1)
                    {
                        await SendLine(client, "Co chesz zmienić?\r\n[0] Nazwę\r\n[1] Adres\r\n[3] Anuluj");
                        int action = await getUserInputInt(client, buffer);
                        //changing name
                        if (action == 0)
                        {
                            await Send(client, "Podaj nową nazwę: ");
                            string value = await getUserInput(client, buffer);
                            if (db.EditLocation(user, true, names[index], value)) await SendLine(client, "Zmiana nazwy zakończona pomyślnie");
                        }
                        //changing address(osmId)
                        if (action == 1)
                        {
                            Address addr = null;
                            string message;
                            while (addr == null)
                            {
                                await Send(client, "Podaj nowy adres: ");
                                message = await getUserInput(client, buffer);
                                addr = await getAddress(client, buffer, message);
                            }
                            string osm = "";
                            switch (addr.OsmType)
                            {
                                case "node":
                                    osm = "N" + addr.OsmId;
                                    break;
                                case "way":
                                    osm = "W" + addr.OsmId;
                                    break;
                                case "relation":
                                    osm = "R" + addr.OsmId;
                                    break;
                            }
                            if (db.EditLocation(user, false, names[index], osm)) await SendLine(client, "Zmiana adresu zakończona pomyślnie");
                            return true;
                        }
                        return false;
                    }

                    //deleting location
                    else if (choice == 2)
                    {
                        if (db.DeleteLocation(user, names[index])) await SendLine(client, "Pomyślnie usunięto lokalizację");
                    }
                }
                else
                {
                    await SendLine(client, $"Użytkownik {user} nie posiada zapisanych lokalizacji");
                    return false;
                }
                return true;
            }
            else if (choice == 3) return true;
            return false;
        }*/
    }
}
