using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Integration
{
    public class MessageBufferTests : TestFixtureBase
    {
        public MessageBufferTests() : base(8080)
        { }

        [Fact]
        public async Task FlushAsync_Transmits_Single_Message_Over_Transport()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = Guid.Empty;
            await OpenTransportAsync();

            MessageBuffer.Write(expectedMessage);
            await MessageBuffer.FlushAsync(CancellationToken.None);
            actualMessage = new Guid(GetReceivedBytes());

            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public async Task FlushAsync_Transmits_Multiple_Messages_Over_Transport()
        {
            var expectedMessages = Enumerable.Range(0, 10).Select(i => Guid.NewGuid()).ToList();
            await OpenTransportAsync();

            expectedMessages.ForEach(m => MessageBuffer.Write(m));
            await MessageBuffer.FlushAsync(CancellationToken.None);

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
