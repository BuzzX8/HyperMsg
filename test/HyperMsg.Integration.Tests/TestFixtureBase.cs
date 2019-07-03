using HyperMsg.Sockets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Integration
{
    public abstract class TestFixtureBase : IDisposable
    {
        protected readonly IPEndPoint EndPoint = new IPEndPoint(IPAddress.Loopback, 8080);
        protected readonly ConfigurableServiceProvider ServiceProvider;

        protected readonly ITransport Transport;
        protected Socket AcceptedSocket;

        private Socket listeningSocket;
        private byte[] receiveBuffer;

        protected TestFixtureBase()
        {
            ServiceProvider = new ConfigurableServiceProvider();
            ServiceProvider.UseCoreServices<Guid>(2048, 2048);
            ServiceProvider.UseSockets(EndPoint);
            ServiceProvider.UseGuidSerializer();

            Transport = ServiceProvider.GetService<ITransport>();

            listeningSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listeningSocket.Bind(EndPoint);
            listeningSocket.Listen(1);

            receiveBuffer = new byte[200];
        }

        protected async Task OpenTransportAsync()
        {
            var acceptTask = Task.Run(() => listeningSocket.Accept());
            await Transport.ProcessCommandAsync(TransportCommand.Open, CancellationToken.None);
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
