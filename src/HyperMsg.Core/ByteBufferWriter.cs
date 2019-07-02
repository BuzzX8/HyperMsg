using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class ByteBufferWriter : IBufferWriter<byte>, IDisposable
    {
        private readonly IMemoryOwner<byte> memoryOwner;
        private readonly AsyncHandler<Memory<byte>> flushHandler;
        private int position;

        public ByteBufferWriter(IMemoryOwner<byte> memoryOwner, AsyncHandler<Memory<byte>> flushHandler)
        {
            this.memoryOwner = memoryOwner ?? throw new ArgumentNullException(nameof(memoryOwner));
            this.flushHandler = flushHandler ?? throw new ArgumentNullException(nameof(flushHandler));
            position = 0;
        }

        private Memory<byte> Memory => memoryOwner.Memory;

        public Memory<byte> CommitedMemory => Memory.Slice(0, position);

        public int AvailableMemory => Memory.Length - position;

        public void Advance(int count)
        {
            if (count > AvailableMemory || count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            position += count;
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            if (Memory.Length < sizeHint || sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (position == 0)
            {
                return Memory;
            }

            return Memory.Slice(position);
        }

        public Span<byte> GetSpan(int sizeHint = 0) => GetMemory(sizeHint).Span;

        public async Task FlushAsync(CancellationToken cancellationToken)
        {
            await flushHandler.Invoke(CommitedMemory, cancellationToken);
            position = 0;
        }

        public void Dispose() => memoryOwner.Dispose();
    }
}
