using System;
using System.Buffers;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Transciever
{
    public class MessageReceiverTests
    {
        [Fact]
        public void Receive_Returns_Fully_Received_Message()
        {
            var expectedMessage = Guid.NewGuid();
            var buffer = new Memory<byte>(new byte[100]);
            var receiver = new MessageReceiver<Guid>(DeserializeGuid, buffer, (b, t) =>
            {
                var bytes = expectedMessage.ToByteArray();
                bytes.CopyTo(buffer);
                return Task.FromResult(bytes.Length);
            });

            var acctualMessage = receiver.Receive();

            Assert.Equal(expectedMessage, acctualMessage);
        }

        private DeserializationResult<Guid> DeserializeGuid(ReadOnlySequence<byte> buffer)
        {
            var bytes = buffer.ToArray();
            return new DeserializationResult<Guid>(bytes.Length, new Guid(bytes));
        }
    }
}
