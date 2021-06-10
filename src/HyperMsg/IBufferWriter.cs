using System;

namespace HyperMsg
{
    public interface IBufferWriter
    {
        void Advance(int count);

        Memory<byte> GetMemory(int sizeHint = 0);

        Span<byte> GetSpan(int sizeHint = 0);
    }
}
