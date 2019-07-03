using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Integration
{
    public class MessageBufferTests : TestFixtureBase
    {
        private readonly IMessageBuffer<Guid> messageBuffer;

        public MessageBufferTests()
        {
            messageBuffer = ServiceProvider.GetService<IMessageBuffer<Guid>>();
        }

        [Fact]
        public async Task FlushAsync_Transmits_Message_Over_Transport()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = Guid.Empty;
            await OpenTransportAsync();

            messageBuffer.Write(expectedMessage);
            await messageBuffer.FlushAsync(CancellationToken.None);
            actualMessage = new Guid(ReceiveMessage());

            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}
