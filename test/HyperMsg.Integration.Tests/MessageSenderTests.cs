using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Integration
{
    public class MessageSenderTests : TestFixtureBase
    {
        private readonly IMessageSender<Guid> messageSender;

        public MessageSenderTests()
        {
            messageSender = ServiceProvider.GetService<IMessageSender<Guid>>();
        }

        [Fact]
        public async Task SendAsync_Transmits_Message_Over_Transport()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = Guid.Empty;
            await OpenTransportAsync();

            await messageSender.SendAsync(expectedMessage, CancellationToken.None);
            var receiveBuffer = new byte[100];
            var received = AcceptedSocket.Receive(receiveBuffer);
            actualMessage = new Guid(new ReadOnlySpan<byte>(receiveBuffer, 0, received));

            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}
