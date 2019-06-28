using System.Buffers;

namespace HyperMsg
{
    public delegate DeserializationResult<T> DeserializeFunc<T>(ReadOnlySequence<byte> buffer);
}