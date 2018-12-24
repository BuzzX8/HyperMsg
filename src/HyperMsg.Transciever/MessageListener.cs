using System;
using System.Buffers;

namespace HyperMsg
{
    public class MessageListener<T>
    {        
        private readonly Func<ReadOnlySequence<byte>, DeserializationResult<T>> deserializer;
        private readonly Action<T> observer;

        public MessageListener(Func<ReadOnlySequence<byte>, DeserializationResult<T>> deserializer, Action<T> observer)
        {            
            this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
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
                observer.Invoke(result.Message);
            }

            return result.BytesConsumed;
        }
    }
}
