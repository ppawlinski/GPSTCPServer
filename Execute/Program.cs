using System.Net;
using GPSTCPServer;

namespace Execute
{
    class Program
    {
        static void Main(string[] args)
        {
            Database database = new Database("Data Source=database.sqlite3");
            GPSTCPServer.TCPServerGPS server = new TCPServerGPS(IPAddress.Any, 2048, database);
            server.RunServer().Wait();
        }
    }
}
