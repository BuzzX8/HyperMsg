using System.Net.Sockets;

namespace HyperMsg.Sockets
{
    internal static class SocketFactory
    {
        public static Socket CreateTcpSocket() => new Socket(SocketType.Stream, ProtocolType.Tcp);
    }
}
