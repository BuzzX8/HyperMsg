using System;
using System.Collections.Generic;
using System.Linq;
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
            var expectedMessages = Enumerable.Range(0, 10).Select(i => Guid.NewGuid()).ToList();
            var actualMessages = new List<Guid>();
            var @event = new ManualResetEventSlim();

            var received = 0;
            registry.Register(g =>
            {
                actualMessages.Add(g);
                received++;

                if (received == expectedMessages.Count)
                {
                    @event.Set();
                }
            });

            await OpenTransportAsync();
            expectedMessages.ForEach(g => AcceptedSocket.Send(g.ToByteArray()));
            @event.Wait(waitTimeout);

            Assert.Equal(expectedMessages, actualMessages);
        }
    }
}
