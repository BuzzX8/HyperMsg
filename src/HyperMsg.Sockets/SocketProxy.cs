using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class SocketProxy : ISocket, IDisposable
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

        public bool IsConnected => Socket.Connected;

        public void Connect() => Socket.Connect(endpoint);

        public Task ConnectAsync(CancellationToken token) => Task.Run((Action)Connect);

        public void Disconnect() => Socket.Disconnect(false);

        public Task DisconnectAsync(CancellationToken token) => Task.Run((Action)Disconnect);

        public void Dispose() => Disconnect();

        private Stream CreateStream() => new NetworkStream(Socket);
    }
}
