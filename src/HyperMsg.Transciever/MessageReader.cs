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

        public int ReadBuffer(ReadOnlySequence<byte> buffer)
        {
            throw new NotImplementedException();
        }

        public void SetMessageHandler(Action<T> handler)
        {
            messageHandler = handler;
        }
    }
}
