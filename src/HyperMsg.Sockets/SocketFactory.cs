using System.Net.Sockets;

namespace HyperMsg.Sockets
{
    public static class SocketFactory
    {
        public static ISocket CreateTcpSocket() => new SocketProxy(new Socket(SocketType.Stream, ProtocolType.Tcp));
    }
}
