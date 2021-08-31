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

            Sender.SendActionRequestToReceiveBuffer(buffer => buffer.Writer.Write(expectedData));

            actualData = buffer.Reader.Read().ToArray();
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task SendActionRequestToReceiveBufferAsync_Invokes_Action_For_Specified_Buffer()
        {
            var buffer = GetRequiredService<IBufferContext>().ReceivingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            await Sender.SendActionRequestToReceiveBufferAsync((buffer, _) =>
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

            Sender.SendActionRequestToTransmitBuffer(buffer => buffer.Writer.Write(expectedData));

            actualData = buffer.Reader.Read().ToArray();
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public async Task SendActionRequestToTransmitBufferAsync_Invokes_Action_For_Specified_Buffer()
        {
            var buffer = GetRequiredService<IBufferContext>().TransmittingBuffer;
            var expectedData = Guid.NewGuid().ToByteArray();
            var actualData = default(byte[]);

            await Sender.SendActionRequestToTransmitBufferAsync((buffer, _) =>
            {
                buffer.Writer.Write(expectedData);
                return Task.CompletedTask;
            });

            actualData = buffer.Reader.Read().ToArray();
            Assert.Equal(expectedData, actualData);
        }

        [Fact]
        public void RegisterSerializer_Writes_Transmitted_Data_Into_Buffer()
        {
            var buffer = GetRequiredService<IBufferContext>().TransmittingBuffer;
            var expectedData = Guid.NewGuid();
            var actualData = default(Guid);
            HandlersRegistry.RegisterSerializer<Guid>(Sender, (writer, data) => writer.Write(data.ToByteArray()));

            Sender.SendTransmitCommand(expectedData);
            actualData = new Guid(buffer.Reader.Read().ToArray());

            Assert.Equal(expectedData, actualData);
        }
    }
}