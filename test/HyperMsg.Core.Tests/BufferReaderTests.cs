using System;
using System.Buffers;
using Xunit;

namespace HyperMsg
{
    public class BufferReaderTests
    {
        [Fact]
        public void ReadBuffer_Returns_Consumed_Bytes_And_Calls_Handler()
        {
            var buffer = new ReadOnlySequence<byte>(Guid.NewGuid().ToByteArray());
            var message = Guid.NewGuid().ToString();
            var actualMessage = string.Empty;
            var listener = new BufferReader<string>(b => new DeserializationResult<string>(message.Length, message), m => actualMessage = m);

            var consumed = listener.ReadBuffer(buffer);

            Assert.Equal(message.Length, consumed);
            Assert.Equal(message, actualMessage);
        }
    }
}
