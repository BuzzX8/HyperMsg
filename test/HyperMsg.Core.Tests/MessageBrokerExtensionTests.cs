using FakeItEasy;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageBrokerExtensionTests
    {
        private readonly MessageBroker broker = new();
        
        [Fact]
        public void SendToTransmitBuffer_Invokes_Registered_Serializer()
        {
            var expectedMessage = Guid.NewGuid();
            var actualMessage = Guid.Empty;

            broker.RegisterSerializer<Guid>((writer, message) => writer.Write(message.ToByteArray()));
            broker.RegisterReceiveBufferHandler(buffer => actualMessage = new Guid(buffer.Reader.Read().ToArray()));

            broker.SendToTransmitBuffer(actualMessage);

            Assert.Equal(expectedMessage, actualMessage);
        }
    }
}