using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Integration
{
    public abstract class TestFixtureBase : SocketTransportFixtureBase<Guid>, IDisposable
    {

        private readonly Socket listeningSocket;
        private readonly byte[] receiveBuffer;
        protected Socket AcceptedSocket;

        protected TestFixtureBase(int port) : base(port)
        {
            listeningSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listeningSocket.Bind(EndPoint);
            listeningSocket.Listen(1);

            receiveBuffer = new byte[200];
        }

        protected override void ConfigureSerializer(IConfigurable configurable) => configurable.UseGuidSerializer();

        protected override async Task OpenTransportAsync(CancellationToken cancellationToken = default)
        {
            var acceptTask = Task.Run(() => listeningSocket.Accept(), cancellationToken);
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
