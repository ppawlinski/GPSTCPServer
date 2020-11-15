using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Serialization;
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

        public void RunServer()
        {
            Task.Factory.StartNew(async () =>
            {
                Listener.Start();
                while (true)
                {
                    await Listener.AcceptTcpClientAsync().ContinueWith(async (t) =>
                    {
                        //login loop
                        byte[] buffer = new byte[1024];
                        while (true)
                        {
                            //account creation
                            bool accountCreated = false;
                            while (!accountCreated)
                            {
                                accountCreated = await createAccount(t.Result, buffer);
                            }
                            //login
                            string user = null;
                            while (user == null)
                            {
                                user = await Login(t.Result, buffer);
                            }
                            //main loop until user logs out
                            while (user != null)
                            {
                                user = await mainFunctionality(t.Result, buffer, user);
                            }
                        }

                    });
                }
            });
        }

        private async Task<string> mainFunctionality(TcpClient client, byte[] buffer, string user)
        {
            await SendLine(client, "[0]Nawigacja GPS\r\n[1]Zarządzaj zapisanymi miejscami\r\n[2]Wyloguj");
            int choice = await getUserInputInt(client,buffer);

            if (choice == 0)
            {
                Address addr1 = null;
                string message;
                while (addr1 == null)
                {
                    await Send(client, "Podaj pierwszy adres ([?] Wybierz z zapisanych): ");
                    message = await getUserInput(client, buffer);
                    if (message == "?") addr1 = await getSavedAddress(client, buffer, user);
                    else addr1 = await getAddress(client, buffer, message);
                }
                Address addr2 = null;
                while (addr2 == null)
                {
                    await Send(client, "Podaj drugi adres ([?] Wybierz z zapisanych): ");
                    message = await getUserInput(client, buffer);
                    if (message == "?") addr2 = await getSavedAddress(client, buffer, user);
                    else addr2 = await getAddress(client, buffer, message);
                }
                await SendLine(client, addr1.Lon + ":" + addr1.Lat);
                await SendLine(client, addr2.Lon + ":" + addr2.Lat);
                await getRoute(client, addr1, addr2);
                return user;
            }
            else if (choice == 1)
            {
                while (!await manageSavedLocations(client, buffer, user)) ;
                return user;
            }
            else if (choice == 2) return null;

            return user;
        }


        private async Task<bool> manageSavedLocations(TcpClient client, byte[] buffer, string user)
        {
            await SendLine(client, "[0] Dodaj nowe miejsce\r\n[1] Edytuj zapisane miejsce\r\n[2] Usuń zapisane miejsce\r\n[3] Cofnij");
            int choice = await getUserInputInt(client, buffer);
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
        }

        private async Task<string> getUserInput(TcpClient client, byte[] buffer)
        {
            Array.Clear(buffer, 0, buffer.Length);
            await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
            await client.GetStream().ReadAsync(new byte[10]);
            return Encoding.UTF8.GetString(buffer).Trim().Replace("\0", String.Empty);
        }

        private async Task<int> getUserInputInt(TcpClient client, byte[] buffer)
        {
            string input = await getUserInput(client, buffer);
            int number;
            try
            {
                number = int.Parse(input);
            }
            catch (Exception)
            {
                return -1;
            }
            return number;
        }

        private async Task<bool> createAccount(TcpClient client, byte[] buffer)
        {
            await SendLine(client, "[0] Zaloguj\r\nlub\r\n[1] Stwórz konto");
            int c = await getUserInputInt(client, buffer);
            if (c == 1)
            {
                string username, p1 = null, p2 = null;
                do
                {
                    if (p1 != null) await SendLine(client, "Hasła muszą być takie same!");
                    await Send(client, "Podaj nazwę użytkownika: ");
                    username = await getUserInput(client, buffer);
                    await Send(client, "Podaj hasło: ");
                    p1 = await getUserInput(client, buffer);
                    await Send(client, "Powtórz hasło: ");
                    p2 = await getUserInput(client, buffer);
                } while (p1 != p2);


                if (db.CreateUser(username, p1))
                {
                    await SendLine(client, "Konto utworzone pomyślnie.");
                    return true;
                }
                await SendLine(client, "Użytkownik o podanej nazwie już istnieje!");
                return false;
            }
            else if (c == 0) { return true; }
            return false;
        }

        private async Task<string> Login(TcpClient client, byte[] buffer)
        {
            string username, password;
            await Send(client, "Podaj nazwę użytkownika: ");
            username = await getUserInput(client, buffer);

            await Send(client, "Podaj hasło: ");
            password = await getUserInput(client, buffer);

            if (db.UserLogin(username, password))
            {
                await SendLine(client, $"Pomyślnie zalogowano jako {username}");
                return username;
            }

            await SendLine(client, "Nieprawidłowa nazwa użytkownika lub hasło!");
            return null;
        }

        public static async Task Send(TcpClient client, string message)
        {
            await client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(message));
        }

        public static async Task SendLine(TcpClient client, string message)
        {
            await client.GetStream().WriteAsync(Encoding.UTF8.GetBytes(message + Environment.NewLine));
        }

        private async Task<Address> getAddress(TcpClient client, byte[] buffer, string message)
        {
            Address[] address = JsonSerializer.Deserialize<Address[]>(await GetRequest.GetFromURLAsync(String.Format("https://nominatim.openstreetmap.org/search?q={0}&format=json", message.Replace(" ", "+"))));
            if (address.Length == 0)
            {
                await SendLine(client, "Nie znaleziono adresu");
                return null;
            }
            else if (address.Length == 1)
            {
                return address[0];
            }
            else if (address.Length > 1)
            {
                await SendLine(client, "Znaleziono " + address.Length + " adresow.");
                for (int i = 0; i < address.Length; i++)
                {
                    await SendLine(client, "[" + i + "]: " + address[i].DisplayName);
                }
                int choice = -1;
                while (choice < 0 || choice >= address.Length || choice == -1)
                {
                    await Send(client, "(" + 0 + "-" + (address.Length - 1) + "), c - zmień adres: ");
                    message = await getUserInput(client, buffer);
                    try
                    {
                        choice = int.Parse(message);
                    }
                    catch (FormatException)
                    {
                        if (message == "c") return null;
                    }
                    catch (Exception) { }
                }
                return address[choice];
            }
            else throw new Exception();
        }

        private async Task<Address> getSavedAddress(TcpClient client, byte[] buffer, string user)
        {
            var names = db.getUserLocations(user);
            if (names != null)
            {
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
                    if (index == names.Count) return null;
                    index = await getUserInputInt(client, buffer);
                }
                string osmID = db.GetAddress(user, names[index]);
                Address[] address = JsonSerializer.Deserialize<Address[]>(await GetRequest.GetFromURLAsync(String.Format("https://nominatim.openstreetmap.org/lookup?osm_ids={0}&format=json", osmID)));
                return address[0];

            }
            else await SendLine(client, $"Użytkownik {user} nie posiada zapisanych lokalizacji");
            return null;
        }

        private async Task getRoute(TcpClient client, Address origin, Address destination)
        {

            RouterCalculator calculator = new RouterCalculator(origin, destination);
            if (!calculator.OK)
            {
                await SendLine(client, "Nie można znaleźć połączenia między podanymi punktami");
                return;
            }
            string[] instructions = calculator.getInstructions();
            foreach (var instruction in instructions)
            {
                if (instruction != String.Empty) await SendLine(client, instruction);
            }
        }


    }
}
