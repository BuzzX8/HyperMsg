using System.Net.Sockets;

namespace HyperMsg.Sockets
{
    public static class SocketFactory
    {
        public static Socket CreateTcpSocket() => new Socket(SocketType.Stream, ProtocolType.Tcp);
    }
}
