using System;
using System.Buffers;

namespace HyperMsg
{
    public class ByteBufferWriter : IBufferWriter<byte>
    {
        private readonly IMemoryOwner<byte> memoryOwner;
        private int position;

        public ByteBufferWriter(IMemoryOwner<byte> memoryOwner)
        {
            this.memoryOwner = memoryOwner ?? throw new ArgumentNullException(nameof(memoryOwner));
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

        public void Reset() => position = 0;

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
