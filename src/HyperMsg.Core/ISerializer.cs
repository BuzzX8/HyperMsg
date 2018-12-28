using System.Buffers;

namespace HyperMsg
{
    public interface ISerializer<T>
    {
        DeserializationResult<T> Deserialize(ReadOnlySequence<byte> buffer);

        void Serialize(IBufferWriter<byte> writer, T message);
    }
}