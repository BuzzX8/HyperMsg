using System;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct DeserializationResult<T> : IEquatable<DeserializationResult<T>>
    {
        public DeserializationResult(int bytesConsumed, T message)
        {
            if (bytesConsumed < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesConsumed));
            }

            MessageSize = bytesConsumed;
            Message = message;
        }

        /// <summary>
        /// 
        /// </summary>
        public int MessageSize { get; }

        /// <summary>
        /// 
        /// </summary>
        public T Message { get; }

        public override int GetHashCode() => MessageSize.GetHashCode() ^ Message.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(DeserializationResult<T>))
            {
                return false;
            }

            return Equals((DeserializationResult<T>)obj);
        }

        public bool Equals(DeserializationResult<T> other)
        {
            if (Message != default && !Message.Equals(other.Message))
            {
                return false;
            }

            return MessageSize.Equals(other.MessageSize);
        }

        public override string ToString() => $"{nameof(MessageSize)}:{MessageSize};{nameof(Message)}:{Message}";
    }
}
