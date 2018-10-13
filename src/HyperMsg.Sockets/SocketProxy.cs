using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Sockets
{
    public class SocketProxy : IDisposable
    {
        private readonly Lazy<Socket> socket;
        private readonly Lazy<Stream> stream;
        private readonly EndPoint endpoint;

        public SocketProxy(Func<Socket> socketFactory, EndPoint endpoint)
        {
            socket = new Lazy<Socket>(socketFactory);
            stream = new Lazy<Stream>(CreateStream);
            this.endpoint = endpoint;
        }

        internal SocketProxy(Socket socket)
        {
            this.socket = new Lazy<Socket>(() => socket);
            stream = new Lazy<Stream>(CreateStream);
        }

        protected Socket Socket => socket.Value;

        public Stream Stream => stream.Value;

        public void Connect() => Socket.Connect(endpoint);

        public void Disconnect() => Socket.Disconnect(false);

        public void Dispose() => Disconnect();

        private Stream CreateStream() => new NetworkStream(Socket);
    }
}
