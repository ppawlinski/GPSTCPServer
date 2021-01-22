using System.Net;
using GPSTCPServer;

namespace Execute
{
    class Program
    {
        static void Main(string[] args)
        {
            GPSTCPServer.TCPServerGPS server = new TCPServerGPS(IPAddress.Any, 2048);
            server.RunServer().Wait();
        }
    }
}
