using System;
using System.Buffers;
using Xunit;

namespace HyperMsg
{
    public class BufferServiceTests : ServiceHostFixture
    {
        [Fact]
        public void SendWriteToBufferCommand_()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = default(Guid?);

            MessageSender.SendWriteToBufferCommand(expectedMessage, BufferType.Transmitting);
            MessageSender.SendBufferActionRequest(buffer => 
            {
                var bytes = buffer.Reader.Read().ToArray();
                actualMessage = new Guid(bytes);
            }, BufferType.Transmitting);

            Assert.NotNull(actualMessage);
            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}
