using System;
using System.Buffers;
using Xunit;

namespace HyperMsg
{
    public class BufferServiceTests : ServiceHostFixture
    {
        [Fact]
        public void SendWriteToBufferCommand_Writes_Message_To_Specified_Buffer()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = default(Guid?);

            HandlersRegistry.RegisterSerializationHandler<Guid>((buffer, m) =>
            {
                buffer.Write(m.ToByteArray());
            });
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
