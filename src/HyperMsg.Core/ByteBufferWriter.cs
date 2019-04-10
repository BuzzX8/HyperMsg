using System;
using System.Buffers;

namespace HyperMsg
{
    public class ByteBufferWriter : IBufferWriter<byte>
    {
        private readonly Memory<byte> buffer;
        private int position;

        public ByteBufferWriter(Memory<byte> buffer)
        {
            this.buffer = buffer;
            position = 0;
        }

        private Memory<byte> Memory => buffer;

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
