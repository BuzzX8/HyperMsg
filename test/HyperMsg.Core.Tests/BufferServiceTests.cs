using System;
using System.Buffers;
using Xunit;

namespace HyperMsg
{
    public class BufferServiceTests : ServiceHostFixture
    {
        [Fact]
        public void SendWriteToBufferCommand_Writes_Message_To_Specified_Buffer_With_Serializer()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = default(Guid?);

            HandlersRegistry.RegisterSerializationHandler<Guid>((buffer, m) => buffer.Write(m.ToByteArray()));
            MessageSender.SendWriteToBufferCommand(expectedMessage, BufferType.Transmitting);
            MessageSender.SendBufferActionRequest(buffer => actualMessage = new Guid(buffer.Reader.Read().ToArray()), BufferType.Transmitting);

            Assert.NotNull(actualMessage);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void SendWriteToBufferCommand_Writes_Binary_Data_To_Specified_Buffer_Without_Serializer()
        {
            var expectedMessage = Guid.NewGuid().ToByteArray();
            var actualMessage = default(byte[]);
                        
            MessageSender.SendWriteToBufferCommand(expectedMessage, BufferType.Receiving);
            MessageSender.SendBufferActionRequest(buffer => actualMessage = buffer.Reader.Read().ToArray(), BufferType.Receiving);

            Assert.NotNull(actualMessage);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void Invokes_Handler_Registered_By_RegisterBufferUpdateEventHandler()
        {
            var expectedMessage = Guid.NewGuid().ToByteArray();
            var actualMessage = default(byte[]);

            HandlersRegistry.RegisterBufferUpdateEventHandler(BufferType.Transmitting, buffer => actualMessage = buffer.Reader.Read().ToArray());
            MessageSender.SendWriteToBufferCommand(expectedMessage, BufferType.Transmitting);

            Assert.NotNull(actualMessage);
            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}