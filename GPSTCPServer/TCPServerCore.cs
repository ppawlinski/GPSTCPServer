using System.Net;
using System.Net.Sockets;

namespace GPSTCPServer
{
    public abstract class TCPServerCore
    {
        protected TcpListener Listener { get; set;}
        protected TCPServerCore(IPAddress iP, int port)
        {
            Listener = new TcpListener(iP, port);
        }

    }
}
