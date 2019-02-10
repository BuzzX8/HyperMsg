using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Transciever
{
    public class PipeWriter : IPipeWriter
    {
        private readonly IMemoryOwner<byte> memoryOwner;
        private readonly ReadBufferAction readBufferAction;
        private int position;

        public PipeWriter(IMemoryOwner<byte> memoryOwner, ReadBufferAction readBufferAction)
        {
            this.memoryOwner = memoryOwner ?? throw new ArgumentNullException(nameof(memoryOwner));
            this.readBufferAction = readBufferAction ?? throw new ArgumentNullException(nameof(readBufferAction));
            position = 0;
        }

        private Memory<byte> Memory => memoryOwner.Memory;

        private Memory<byte> CommitedMemory => Memory.Slice(0, position);

        public int AvailableMemory => Memory.Length - position;

        public void Advance(int count)
        {
            if (count > AvailableMemory || count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            position += count;
        }

        public void Flush()
        {
            var buffer = new ReadOnlySequence<byte>(CommitedMemory);
            readBufferAction.Invoke(buffer);
        }

        public Task FlushAsync(CancellationToken token = default)
        {
            Flush();
            return Task.CompletedTask;
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
    }
}
