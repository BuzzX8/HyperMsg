using System;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Sockets
{
    public class SocketProxy : IDisposable
    {
        private readonly Lazy<Socket> socket;        
        private readonly EndPoint endpoint;

        public SocketProxy(Func<Socket> socketFactory, EndPoint endpoint)
        {
            socket = new Lazy<Socket>(socketFactory);
            this.endpoint = endpoint;
        }

        internal SocketProxy(Socket socket)
        {
            this.socket = new Lazy<Socket>(() => socket);
        }

        protected Socket Socket => socket.Value;

        public Stream Stream { get; private set; }

        public void Connect()
        {
            Socket.Connect(endpoint);
            Stream = new NetworkStream(Socket);
        }

        public void Disconnect() => Socket.Disconnect(false);

        public void Dispose() => Disconnect();
    }
}
