using System;
using System.Buffers;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BufferContextTests : HostFixture
    {
        private readonly IBufferContext bufferContext;

        public BufferContextTests() => bufferContext = GetRequiredService<IBufferContext>();        

        [Fact]
        public void TransmittingBuffer_Flush_Sends_Buffered_Data_To_TransmitBufferHandler()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);
            HandlersRegistry.RegisterTransmitBufferHandler(reader => actual = reader.Read().ToArray());

            var buffer = bufferContext.TransmittingBuffer;
            buffer.Writer.Write(expected);
            buffer.Flush();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task TransmittingBuffer_FlushAsync_Sends_Buffered_Data_To_TransmitBufferHandler()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);
            HandlersRegistry.RegisterTransmitBufferHandler(reader => actual = reader.Read().ToArray());

            var buffer = bufferContext.TransmittingBuffer;
            buffer.Writer.Write(expected);
            await buffer.FlushAsync(default);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ReceivingBuffer_Flush_Sends_Buffered_Data_To_TransmitBufferHandler()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);
            HandlersRegistry.RegisterReceiveBufferHandler(reader => actual = reader.Read().ToArray());

            var buffer = bufferContext.ReceivingBuffer;
            buffer.Writer.Write(expected);
            buffer.Flush();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task ReceivingBuffer_FlushAsync_Sends_Buffered_Data_To_TransmitBufferHandler()
        {
            var expected = Guid.NewGuid().ToByteArray();
            var actual = default(byte[]);
            HandlersRegistry.RegisterReceiveBufferHandler(reader => actual = reader.Read().ToArray());

            var buffer = bufferContext.ReceivingBuffer;
            buffer.Writer.Write(expected);
            await buffer.FlushAsync(default);

            Assert.Equal(expected, actual);
        }
    }
}