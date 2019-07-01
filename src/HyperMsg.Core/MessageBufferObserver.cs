using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageBufferObserver<T>
    {
        private readonly DeserializeFunc<T> deserialize;
        private readonly IBufferReader bufferReader;
        private readonly int deserializeInvokeCount;

        private const int DefaultDeserializeInvokeCount = 10;

        public MessageBufferObserver(DeserializeFunc<T> deserialize, IBufferReader bufferReader, int deserializeInvokeCount = DefaultDeserializeInvokeCount)
        {
            this.deserialize = deserialize ?? throw new ArgumentNullException(nameof(deserialize));
            this.bufferReader = bufferReader ?? throw new ArgumentNullException(nameof(bufferReader));
            this.deserializeInvokeCount = deserializeInvokeCount;
        }

        public async Task CheckBufferAsync(CancellationToken cancellationToken)
        {
            var buffer = await bufferReader.ReadAsync(cancellationToken);
            var result = default(DeserializationResult<T>);
            var deserializeSize = 0;

            for (int i = 0; i < deserializeInvokeCount; i++)
            {
                result = deserialize(buffer);
                deserializeSize += result.MessageSize;

                if (result.MessageSize == 0)
                {
                    break;
                }

                OnMessageDeserialized(result.Message);
            }

            bufferReader.Advance(deserializeSize);
        }

        private void OnMessageDeserialized(T message)
        {
            MessageDeserialized?.Invoke(message);
        }

        public event Action<T> MessageDeserialized;
    }
}
