using System.Buffers;
using Xunit;

namespace HyperMsg
{
    public class BufferFactoryTests
    {
        private readonly BufferFactory factory = new BufferFactory(MemoryPool<byte>.Shared);

        [Fact]
        public void CreateBuffer_Creates_Buffer()
        {            
            var bufferSize = 50;

            var buffer = factory.CreateBuffer(bufferSize);

            Assert.NotNull(buffer);
        }

        [Fact]
        public void CreateContext_Creates_BufferContext()
        {
            var context = factory.CreateContext(100, 100);

            Assert.NotNull(context);
            Assert.NotNull(context.ReceivingBuffer);
            Assert.NotNull(context.TransmittingBuffer);
        }
    }
}
