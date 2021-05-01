using System;
using System.Buffers;
using Xunit;

namespace HyperMsg
{
    public class BufferServiceTests : ServiceHostFixture
    {
        [Fact]
        public void SendReadFromBufferCommand_Provides_Buffer_Data_To_BufferReader()
        {
            var expectedMessage = Guid.NewGuid().ToByteArray();
            var actualMessage = default(byte[]);

            MessageSender.SendWriteToBufferCommand(BufferType.Receiving, expectedMessage);
            MessageSender.SendReadFromBufferCommand(BufferType.Receiving, ros =>
            {
                actualMessage = ros.ToArray();
                return actualMessage.Length;
            });

            Assert.NotNull(actualMessage);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void SendWriteToBufferCommand_Writes_Message_To_Specified_Buffer_With_Serializer()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = default(Guid?);

            HandlersRegistry.RegisterSerializationHandler<Guid>((buffer, m) => buffer.Write(m.ToByteArray()));
            MessageSender.SendWriteToBufferCommand(BufferType.Transmitting, expectedMessage);
            MessageSender.SendBufferActionRequest(buffer => actualMessage = new Guid(buffer.Reader.Read().ToArray()), BufferType.Transmitting);

            Assert.NotNull(actualMessage);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void SendWriteToBufferCommand_Writes_Binary_Data_To_Specified_Buffer_Without_Serializer()
        {
            var expectedMessage = Guid.NewGuid().ToByteArray();
            var actualMessage = default(byte[]);
                        
            MessageSender.SendWriteToBufferCommand(BufferType.Receiving, expectedMessage);
            MessageSender.SendBufferActionRequest(buffer => actualMessage = buffer.Reader.Read().ToArray(), BufferType.Receiving);

            Assert.NotNull(actualMessage);
            Assert.Equal(expectedMessage, actualMessage);
        }

        [Fact]
        public void SendWriteToBufferCommand_Invokes_Handler_Registered_By_RegisterBufferUpdateEventHandler()
        {
            var wasInvoked = false;

            HandlersRegistry.RegisterBufferUpdateEventHandler(BufferType.Transmitting, () => wasInvoked = true);
            MessageSender.SendWriteToBufferCommand(BufferType.Transmitting, Guid.NewGuid().ToByteArray());

            Assert.True(wasInvoked);
        }
    }
}