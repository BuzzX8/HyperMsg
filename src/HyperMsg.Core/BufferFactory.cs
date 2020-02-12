using System;
using System.Buffers;

namespace HyperMsg
{
    public class BufferFactory : IBufferFactory, IDisposable
    {
        private readonly MemoryPool<byte> memoryPool;

        public BufferFactory(MemoryPool<byte> memoryPool)
        {
            this.memoryPool = memoryPool ?? throw new ArgumentNullException(nameof(memoryPool));
        }

        public IBuffer CreateBuffer(int bufferSize)
        {
            var memory = memoryPool.Rent(bufferSize);
            return new Buffer(memory);
        }

        public IBufferContext CreateContext(int receivingBufferSize, int transmittingBufferSize)
        {
            var receivingBuffer = new Buffer(memoryPool.Rent(receivingBufferSize));
            var transmittingBuffer = new Buffer(memoryPool.Rent(transmittingBufferSize));

            return new BufferContext(receivingBuffer, transmittingBuffer);
        }

        public void Dispose() => memoryPool.Dispose();
    }
}
