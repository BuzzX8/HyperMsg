using System;
using System.Buffers;

namespace HyperMsg.Transciever
{
    public class MessageNotifier<T> : IReceiveNotifier<T>
    {
        private readonly DeserializeFunc<T> deserializeFunc;

        public MessageNotifier(DeserializeFunc<T> deserializeFunc)
        {
            this.deserializeFunc = deserializeFunc ?? throw new ArgumentNullException(nameof(deserializeFunc));
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

            OnMessageReceived(result.Message);

            return result.BytesConsumed;
        }

        private void OnMessageReceived(T message)
        {
            Received?.Invoke(message);
        }

        public event Action<T> Received;
    }
}
