using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageBufferObserver<T>
    {
        private readonly DeserializeFunc<T> deserialize;
        private readonly IBufferReader<byte> bufferReader;
        private readonly int deserializeInvokeCount;

        private const int DefaultDeserializeInvokeCount = 10;

        public MessageBufferObserver(DeserializeFunc<T> deserialize, IBufferReader<byte> bufferReader, int deserializeInvokeCount = DefaultDeserializeInvokeCount)
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

                if (result.MessageSize > buffer.Length)
                {
                    throw new DeserializationException();
                }

                deserializeSize += result.MessageSize;

                if (result.MessageSize == 0)
                {
                    break;
                }

                buffer = buffer.Slice(result.MessageSize);
                await OnMessageDeserializedAsync(result.Message, cancellationToken);
            }

            bufferReader.Advance(deserializeSize);
        }

        private async Task OnMessageDeserializedAsync(T message, CancellationToken cancellationToken)
        {
            if (MessageDeserialized != null)
            {
                await MessageDeserialized?.Invoke(message, cancellationToken);
            }
        }

        public event AsyncAction<T> MessageDeserialized;
    }
}
