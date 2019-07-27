using HyperMsg.Sockets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Integration
{
    public abstract class SocketFixtureBase<T> : IntegrationFixtureBase<T>, IDisposable
    {
        private readonly IPEndPoint endPoint;
        private readonly Socket listeningSocket;
        private readonly byte[] receiveBuffer;

        protected Socket AcceptedSocket;

        protected SocketFixtureBase(int port) : this(IPAddress.Loopback, port)
        { }

        protected SocketFixtureBase(IPAddress address, int port)
        {
            endPoint = new IPEndPoint(address, port);

            listeningSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listeningSocket.Bind(endPoint);
            listeningSocket.Listen(1);

            receiveBuffer = new byte[200];
        }

        protected override void ConfigureTransport(IConfigurable configurable) => configurable.UseSockets(endPoint);

        protected override async Task OpenTransportAsync(CancellationToken cancellationToken = default)
        {
            var acceptTask = Task.Run(() => listeningSocket.Accept());
            await base.OpenTransportAsync(cancellationToken);
            AcceptedSocket = await acceptTask;
        }

        protected ReadOnlySpan<byte> GetReceivedBytes()
        {
            var received = AcceptedSocket.Receive(receiveBuffer);
            return new ReadOnlySpan<byte>(receiveBuffer, 0, received);
        }

        public void Dispose()
        {
            listeningSocket.Close();
            AcceptedSocket?.Close();
        }
    }
}
