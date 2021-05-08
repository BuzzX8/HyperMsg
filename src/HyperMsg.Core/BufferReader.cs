using System.Buffers;

namespace HyperMsg
{
    public delegate int BufferReader(ReadOnlySequence<byte> buffer);
}
