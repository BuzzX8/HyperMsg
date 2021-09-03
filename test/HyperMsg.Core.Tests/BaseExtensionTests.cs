using System;
using System.Buffers;
using Xunit;

namespace HyperMsg
{
    public class BaseExtensionTests : HostFixture
    {
        private readonly MessageBroker broker = new();
        
        [Fact]
        public void SendToTransmitBuffer_Invokes_Registered_Serializer()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = Guid.Empty;

            HandlersRegistry.RegisterSerializer<Guid>((writer, message) => writer.Write(message.ToByteArray()));
            HandlersRegistry.RegisterTransmitBufferHandler(buffer =>
            {
                actualMessage = new Guid(buffer.Reader.Read().ToArray());
            });

            Sender.SendToTransmitBuffer(actualMessage);

            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}