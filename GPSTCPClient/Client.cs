using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
namespace GPSTCPClient
{
    public static class Client
    {
        public static TcpClient TCP { get; set; }
        private static TcpListener Listener { get; set; }
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
        public async static Task<bool> Login(string login, string md5Password)
        {
            await Send($"LOGIN `{login}` `{md5Password}`");
            byte[] buffer = new byte[1024];
            string result = "";
            await Listener.AcceptTcpClientAsync().ContinueWith(async (listen) =>
            {
               result = await getUserInput(new byte[1024]);
            });
            return result == "SUCCESS";
        }

        public async static Task<bool> Register(string login, string md5Password)
        {
            await Send($"REGISTER `{login}` `{md5Password}`");
            byte[] buffer = new byte[1024];
            string result = "";
            await Listener.AcceptTcpClientAsync().ContinueWith(async (listen) =>
            {
                result = await getUserInput(new byte[1024]);
            });
            return result == "SUCCESS";
        }
        public async static Task Disconnect()
        {
            TCP.Close();
            TCP.Dispose();
            TCP = null;
            Listener = null;
        }
        private async static Task<string> getUserInput(byte[] buffer)
        {
            Array.Clear(buffer, 0, buffer.Length);
            await TCP.GetStream().ReadAsync(buffer, 0, buffer.Length);
            await TCP.GetStream().ReadAsync(new byte[10]);
#if DEBUG 
            Console.WriteLine(Encoding.UTF8.GetString(buffer).Trim().Replace("\0", String.Empty));
#endif
            return Encoding.UTF8.GetString(buffer).Trim().Replace("\0", String.Empty);
        }
    }
}
