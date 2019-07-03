using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Integration
{
    [Collection("Integration")]
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
            actualMessage = new Guid(GetReceivedBytes());

            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}
