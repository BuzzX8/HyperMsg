using System;
using System.Buffers;

namespace HyperMsg
{
    public class MessageListener<T>
    {        
        private readonly Func<ReadOnlySequence<byte>, DeserializationResult<T>> deserializer;
        private readonly Action<T> handler;

        public MessageListener(Func<ReadOnlySequence<byte>, DeserializationResult<T>> deserializer, Action<T> handler)
        {            
            this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

		public int ReadBuffer(ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsEmpty)
            {
                return 0;
            }

            var result = deserializer(buffer);

            if (result.Message != null)
            {
                handler.Invoke(result.Message);
            }

            return result.BytesConsumed;
        }
    }
}
