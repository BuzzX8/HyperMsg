using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    internal class SocketProxy : ISocket, IDisposable
    {
        private readonly Socket socket;
        private readonly Lazy<Stream> stream;
        private readonly EndPoint endpoint;

        public SocketProxy(Socket socket, EndPoint endpoint)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            stream = new Lazy<Stream>(CreateStream);
            this.endpoint = endpoint;
        }

        public Stream Stream => stream.Value;

        public bool IsConnected => socket.Connected;

        public void Connect() => socket.Connect(endpoint);

        public Task ConnectAsync(CancellationToken token) => Task.Run((Action)Connect);

        public void Disconnect() => socket.Disconnect(false);

        public Task DisconnectAsync(CancellationToken token) => Task.Run((Action)Disconnect);

        public void Dispose() => Disconnect();

        private Stream CreateStream() => new NetworkStream(socket);
    }
}
