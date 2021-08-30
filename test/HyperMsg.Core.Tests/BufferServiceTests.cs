using System;
using System.Buffers;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BufferServiceTests : HostFixture
    {
        [Fact]
        public void SendActionRequestToReceiveBuffer_Invokes_Action_For_Specified_Buffer()
        {
            var buffer = GetRequiredService<IBufferContext>().ReceivingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            MessageSender.SendActionRequestToReceiveBuffer(buffer => buffer.Writer.Write(expectedData));

            actualData = buffer.Reader.Read().ToArray();
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task SendActionRequestToReceiveBufferAsync_Invokes_Action_For_Specified_Buffer()
        {
            var buffer = GetRequiredService<IBufferContext>().ReceivingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            await MessageSender.SendActionRequestToReceiveBufferAsync((buffer, _) =>
            {
                buffer.Writer.Write(expectedData);
                return Task.CompletedTask;
            });

            actualData = buffer.Reader.Read().ToArray();
            Assert.Equal(expectedData, actualData);
        }
        
        [Fact]
        public void SendActionRequestToTransmitBuffer_Invokes_Action_For_Specified_Buffer()
        {
            var buffer = GetRequiredService<IBufferContext>().TransmittingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            MessageSender.SendActionRequestToTransmitBuffer(buffer => buffer.Writer.Write(expectedData));

            actualData = buffer.Reader.Read().ToArray();
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task SendActionRequestToTransmitBufferAsync_Invokes_Action_For_Specified_Buffer()
        {
            var buffer = GetRequiredService<IBufferContext>().TransmittingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            await MessageSender.SendActionRequestToTransmitBufferAsync((buffer, _) =>
            {
                buffer.Writer.Write(expectedData);
                return Task.CompletedTask;
            });

            actualData = buffer.Reader.Read().ToArray();
            Assert.Equal(expectedData, actualData);
        }
    }
}