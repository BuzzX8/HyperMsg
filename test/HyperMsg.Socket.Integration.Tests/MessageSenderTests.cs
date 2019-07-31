using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Integration
{
    public class MessageSenderTests : TestFixtureBase
    {
        public MessageSenderTests() : base(8082)
        { }

        [Fact]
        public async Task SendAsync_Transmits_Message_Over_Transport()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = Guid.Empty;
            await OpenTransportAsync();

            await MessageSender.SendAsync(expectedMessage, CancellationToken.None);            
            actualMessage = new Guid(GetReceivedBytes());

            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}
