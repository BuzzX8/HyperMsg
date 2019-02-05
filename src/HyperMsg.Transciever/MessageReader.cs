using System;
using System.Buffers;

namespace HyperMsg.Transciever
{
    public class MessageReader<T>
    {
        private readonly Func<ReadOnlySequence<byte>, DeserializationResult<T>> deserializeFunc;
        private Action<T> messageHandler;

        public MessageReader(Func<ReadOnlySequence<byte>, DeserializationResult<T>> deserializeFunc)
        {
            this.deserializeFunc = deserializeFunc ?? throw new ArgumentNullException(nameof(deserializeFunc));
            messageHandler = null;
        }

        public long ReadBuffer(ReadOnlySequence<byte> buffer)
        {
            var result = deserializeFunc.Invoke(buffer);

            if (result.BytesConsumed < 0)
            {
                throw new InvalidOperationException();
            }

            if (result.BytesConsumed == 0)
            {
                return 0;
            }

            messageHandler?.Invoke(result.Message);
            return result.BytesConsumed;
        }

        public void SetMessageHandler(Action<T> handler)
        {
            messageHandler = handler;
        }
    }
}
