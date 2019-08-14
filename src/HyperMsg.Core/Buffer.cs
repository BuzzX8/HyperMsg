using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class Buffer : IBuffer, IBufferReader<byte>, IBufferWriter<byte>, IDisposable
    {
        private readonly IMemoryOwner<byte> memoryOwner;

        private int position;
        private int length;

        public Buffer(IMemoryOwner<byte> memoryOwner)
        {
            this.memoryOwner = memoryOwner ?? throw new ArgumentNullException(nameof(memoryOwner));
        }

        protected Memory<byte> Memory => memoryOwner.Memory;

        public IBufferReader<byte> Reader => this;

        public IBufferWriter<byte> Writer => this;

        private Memory<byte> CommitedMemory => Memory.Slice(0, position);

        private int AvailableMemory => Memory.Length - length;

        void IBufferReader<byte>.Advance(int count)
        {
            if (count > length)
            {
                throw new IndexOutOfRangeException();
            }

            //if (count < position)
            //{
            //    Memory.Slice(count).CopyTo(Memory);
            //}

            position += count;
            length -= count;
        }

        Task<ReadOnlySequence<byte>> IBufferReader<byte>.ReadAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(new ReadOnlySequence<byte>(Memory.Slice(position, length)));
        }

        void IBufferWriter<byte>.Advance(int count)
        {
            if (count > AvailableMemory || count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            length += count;
        }

        Memory<byte> IBufferWriter<byte>.GetMemory(int sizeHint)
        {
            if (Memory.Length < sizeHint || sizeHint < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (length == 0)
            {
                return Memory;
            }

            return Memory.Slice(position, length);
        }

        Span<byte> IBufferWriter<byte>.GetSpan(int sizeHint)
        {
            var writer = (IBufferWriter<byte>)this;
            return writer.GetMemory(sizeHint).Span;
        }

        public void Clear() => throw new NotImplementedException();

        public Task FlushAsync(CancellationToken cancellationToken) => throw new NotImplementedException();

        public void Dispose() => memoryOwner.Dispose();

        public event AsyncAction<IBuffer> FlushRequested;
    }
}