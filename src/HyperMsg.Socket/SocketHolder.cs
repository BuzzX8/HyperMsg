using System.Net.Sockets;

namespace HyperMsg.Socket
{
    public class SocketHolder : IDisposable
    {
        private readonly System.Net.Sockets.Socket socket;

        public SocketHolder(SocketType socketType = SocketType.Stream, ProtocolType protocolType = ProtocolType.Tcp) : this(new(socketType, protocolType))
        { }

        public SocketHolder(System.Net.Sockets.Socket socket)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        public System.Net.Sockets.Socket Socket => socket;

        public void Dispose()
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
            }

            socket.Dispose();
        }
    }
}
