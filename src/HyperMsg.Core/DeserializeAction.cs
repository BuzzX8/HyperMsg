using System.Buffers;

namespace HyperMsg
{
    public delegate (T message, int bytesConsumed) DeserializeAction<T>(ReadOnlySequence<byte> buffer);
}