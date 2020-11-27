using System;
using System.Net.Sockets;

namespace GPSTCPServer
{
    internal class GPSUser
    {
        public TcpClient client;
        public bool LoggedIn { get; set; }
        public string Username { get; set; }

        public GPSUser(TcpClient client)
        {
            this.client = client;
            LoggedIn = false;
            Username = null;
        }

    }
}