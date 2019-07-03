using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Integration
{
    [Collection("Integration")]
    public class MessageHandlerRegistryTests : TestFixtureBase
    {
        private readonly IMessageHandlerRegistry<Guid> registry;
        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        public MessageHandlerRegistryTests()
        {
            registry = ServiceProvider.GetService<IMessageHandlerRegistry<Guid>>();
        }

        [Fact]
        public async Task Registered_Handler_Invoked_When_Message_Received()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = Guid.Empty;
            var @event = new ManualResetEventSlim();
            registry.Register(g =>
            {
                actualMessage = g;
                @event.Set();
            });

            await OpenTransportAsync();
            var sent = AcceptedSocket.Send(expectedMessage.ToByteArray());
            @event.Wait(waitTimeout);

            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}
