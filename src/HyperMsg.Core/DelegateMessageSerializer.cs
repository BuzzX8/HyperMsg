using System;
using System.Buffers;

namespace HyperMsg
{
    public class DelegateMessageSerializer<T> : IMessageSerializer<T>
    {
        private readonly DeserializeFunc<T> deserialize;
        private readonly SerializeAction<T> serialize;

        public DelegateMessageSerializer(DeserializeFunc<T> deserialize, SerializeAction<T> serialize)
        {
            this.deserialize = deserialize ?? throw new ArgumentNullException(nameof(deserialize));
            this.serialize = serialize ?? throw new ArgumentNullException(nameof(serialize));
        }

        public (T Message, int BytesConsumed) Deserialize(ReadOnlySequence<byte> buffer)
        {
            return deserialize(buffer);
        }

        public void Serialize(IBufferWriter<byte> writer, T message)
        {
            serialize(writer, message);
        }
    }
}
