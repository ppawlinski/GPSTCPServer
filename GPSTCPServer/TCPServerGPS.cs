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
                        //loop for logout to work
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
            await Send(client,"[0]Nawigacja GPS\r\n[1]Zarządzaj zapisanymi miejscami\r\n[2]Wyloguj\r\n");
            string message = await getUserInput(client,buffer);
            int choice=-1;
            try
            {
                choice = int.Parse(message);
            }
            catch (Exception) { }

            if (choice == 0)
            {
                Address addr1 = null;
                while (addr1 == null)
                {
                    await Send(client, "Podaj pierwszy adres: ");
                    addr1 = await getAddress(client, buffer);
                }
                await Send(client, "Podaj drugi adres: ");
                Address addr2 = null;
                while (addr2 == null)
                {
                    addr2 = await getAddress(client, buffer);
                }
                await SendLine(client, addr1.Lon + ":" + addr1.Lat);
                await SendLine(client, addr2.Lon + ":" + addr2.Lat);
                await getRoute(client, addr1, addr2);
                return user;
            }
            else if (choice == 1)
            {
                while(!await manageSavedLocations(client, buffer, user));
                return user;
            }
            else if (choice == 2) return null;

            return user;
        }

        private async Task<bool> manageSavedLocations(TcpClient client, byte[] buffer, string user)
        {
            await Send(client,"[0] Dodaj nowe miejsce\r\n[1] Usuń zapisane miejsce\r\n[2] Cofnij\r\n");
            string message = await getUserInput(client, buffer);
            int choice = -1;
            try
            {
                choice = int.Parse(message);
            }
            catch (Exception) { }
            if (choice == 0)
            {
                Address addr = null;
                while (addr == null)
                {
                    await Send(client, "Podaj adres: ");
                    addr = await getAddress(client, buffer);
                }

                await Send(client, "Podaj nazwę pod jaką chcesz zapisać lokalizację: \n");
                string name = await getUserInput(client,buffer);
                //ten if blokuje bazę
                if(!db.AddLocation(user,addr.OsmType,addr.OsmId, name))
                {
                    await Send(client, "Nie udało się zapisać lokalizacji w bazie!\n");
                    return false;
                }
                await Send(client,"Pomyślnie zapisano lokalizację.\n");
                return true;
            }
            else if (choice == 1)
            {
                return true;
            }
            else if (choice == 2) return true;


            return false;
        }

        private async Task<string> getUserInput(TcpClient client, byte[] buffer)
        {
            Array.Clear(buffer, 0, buffer.Length);
            await client.GetStream().ReadAsync(buffer, 0, buffer.Length);
            await client.GetStream().ReadAsync(new byte[10]);
            return Encoding.UTF8.GetString(buffer).Trim().Replace("\0", String.Empty);
        }

        private async Task<bool> createAccount(TcpClient client, byte[] buffer)
        {
            await Send(client, "[0] Zaloguj\r\nlub\r\n[1] Stwórz konto\r\n");
            string choice = await getUserInput(client, buffer);
            int c;
            try
            {
                c = int.Parse(choice);
            }
            catch (Exception)
            {
                return false;
            }
            if (c == 1)
            {
                string username, p1 = null, p2 = null;
                do
                {
                    if (p1 != null) await Send(client, "Hasła muszą być takie same!\r\n");
                    await Send(client, "Podaj nazwę użytkownika: ");
                    username = await getUserInput(client, buffer);
                    await Send(client, "Podaj hasło: ");
                    p1 = await getUserInput(client, buffer);
                    await Send(client, "Powtórz hasło: ");
                    p2 = await getUserInput(client, buffer);
                } while (p1 != p2);


                if (db.CreateUser(username, p1))
                {
                    await Send(client, "Konto utworzone pomyślnie.\r\n");
                    return true;
                }
                await Send(client, "Użytkownik o podanej nazwie już istnieje!\r\n");
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
                await Send(client, $"Pomyślnie zalogowano jako {username}\r\n");
                return username;
            }

            await Send(client, "Nieprawidłowa nazwa użytkownika lub hasło!\r\n");
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

        private async Task<Address> getAddress(TcpClient client, byte[] buffer)
        {

            string message = await getUserInput(client, buffer);
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
