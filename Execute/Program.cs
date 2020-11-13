using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using GPSTCPServer;
using GPSTCPServer;

namespace Execute
{
    class Program
    {
        static void Main(string[] args)
        {
            Database database = new Database();
            GPSTCPServer.TCPServerGPS server = new TCPServerGPS(IPAddress.Any, 2048, database);
            server.RunServer();
            while (true) ;
        }
    }
}
