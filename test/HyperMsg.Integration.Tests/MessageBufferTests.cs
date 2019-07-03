using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Integration
{
    [Collection("Integration")]
    public class MessageBufferTests : TestFixtureBase
    {
        private readonly IMessageBuffer<Guid> messageBuffer;

        public MessageBufferTests()
        {
            messageBuffer = ServiceProvider.GetService<IMessageBuffer<Guid>>();
        }

        [Fact]
        public async Task FlushAsync_Transmits_Single_Message_Over_Transport()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = Guid.Empty;
            await OpenTransportAsync();

            messageBuffer.Write(expectedMessage);
            await messageBuffer.FlushAsync(CancellationToken.None);
            actualMessage = new Guid(GetReceivedBytes());

            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void FlushAsync_Transmits_Multiple_Messages_Over_Transport()
        {
            var expectedMessages = Enumerable.Range(0, 10).Select(i => Guid.NewGuid()).ToList();
            OpenTransportAsync().Wait();

            expectedMessages.ForEach(m => messageBuffer.Write(m));
            messageBuffer.FlushAsync(CancellationToken.None).Wait();

            var actualMessages = DeserializeGuids(GetReceivedBytes());

            Assert.Equal(expectedMessages, actualMessages);
        }

        private Guid[] DeserializeGuids(ReadOnlySpan<byte> buffer)
        {
            var guids = new List<Guid>();

            while (buffer.Length > 0)
            {
                guids.Add(new Guid(buffer.Slice(0, 16)));
                buffer = buffer.Slice(16);
            }

            return guids.ToArray();
        }
    }
}
