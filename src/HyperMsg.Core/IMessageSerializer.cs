using System.Buffers;

namespace HyperMsg
{
    public interface IMessageSerializer<T>
    {
        (T Message, int BytesConsumed) Deserialize(ReadOnlySequence<byte> buffer);

        void Serialize(IBufferWriter<byte> writer, T message);
    }
}
