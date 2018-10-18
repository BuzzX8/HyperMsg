using System.Buffers;

namespace HyperMsg
{
    public delegate (T Message, int BytesConsumed) DeserializeFunc<T>(ReadOnlySequence<byte> buffer);
}