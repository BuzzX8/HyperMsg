using HyperMsg.Sockets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Integration
{
    public class CoreIntegrationTests : IDisposable
    {
        private readonly ConfigurableServiceProvider serviceProvider;
        private readonly ConnectionListener listener;
        private readonly IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 8080);
        private readonly ManualResetEventSlim receiveEvent = new ManualResetEventSlim();

        private ITransport transport;
        private Socket acceptedSocket;
        private Guid receivedMessage;        

        public CoreIntegrationTests()
        {            
            serviceProvider = new ConfigurableServiceProvider();
            serviceProvider.UseCoreServices<Guid>(2048, 2048);
            serviceProvider.UseSockets(endPoint);
            serviceProvider.UseGuidSerializer();
            serviceProvider.RegisterConfigurator((p, s) =>
            {
                var registry = (IMessageHandlerRegistry<Guid>)p.GetService(typeof(IMessageHandlerRegistry<Guid>));
                registry.Register((g, t) =>
                {
                    receivedMessage = g;
                    receiveEvent.Set();
                    return Task.CompletedTask;
                });
            });

            transport = serviceProvider.GetService<ITransport>();
            listener = new ConnectionListener();
        }

        [Fact]
        public async Task TransportOpen_Establishes_Connection_With_Target()
        {
            await OpenTransportAndListenAsync();

            Assert.NotNull(acceptedSocket);
        }

        [Fact]
        public async Task SendMessage_Sends_Serialized_Message()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = Guid.Empty;
            var messageSender = serviceProvider.GetService<IMessageSender<Guid>>();

            await OpenTransportAndListenAsync();
            await messageSender.SendAsync(expectedMessage, CancellationToken.None);

            var receiveBuffer = new byte[16];
            var received = acceptedSocket.Receive(receiveBuffer);

            Assert.Equal(expectedMessage, new Guid(receiveBuffer));
        }

        [Fact]
        public async Task Invokes_MesageHandler_For_Received_Message()
        {
            var expectedMessage = Guid.NewGuid();
                        
            await OpenTransportAndListenAsync();
            acceptedSocket.Send(expectedMessage.ToByteArray());
            receiveEvent.Wait(TimeSpan.FromSeconds(2));

            Assert.Equal(expectedMessage, receivedMessage);
        }

        private async Task OpenTransportAndListenAsync()
        {
            var @event = new ManualResetEventSlim();

            listener.ConnectionAccepted += s =>
            {
                acceptedSocket = s;
                @event.Set();
            };
            listener.StartListening(endPoint);

            await transport.ProcessCommandAsync(TransportCommand.Open, CancellationToken.None);
            @event.Wait();
        }

        public void Dispose()
        {
            transport.ProcessCommandAsync(TransportCommand.Close, CancellationToken.None).Wait();
            acceptedSocket?.Close();
            listener.StopListening();
        }
    }
}
