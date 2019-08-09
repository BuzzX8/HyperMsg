using System;
using System.Buffers;

namespace HyperMsg
{
    public class Buffer : IBuffer, IDisposable
    {
        private readonly IMemoryOwner<byte> memoryOwner;

        public Buffer(IMemoryOwner<byte> memoryOwner)
        {
            this.memoryOwner = memoryOwner ?? throw new ArgumentNullException(nameof(memoryOwner));
        }

        public IBufferReader<byte> Reader => throw new NotImplementedException();

        public IBufferWriter<byte> Writer => throw new NotImplementedException();

        public event AsyncAction<IBuffer> Flushed;

        public void Dispose() => memoryOwner.Dispose();
    }
}