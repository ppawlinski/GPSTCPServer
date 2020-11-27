using System;
using System.Net.Sockets;

namespace GPSTCPServer
{
    internal class GPSUser
    {
        public TcpClient client;

        public int random;
        public bool LoggedIn { get; set; }
        public string Username { get; set; }

        public GPSUser(TcpClient client)
        {
            Random rand = new Random();
            this.client = client;
            LoggedIn = false;
            Username = null;
            random = rand.Next();
        }

    }
}