using System;

namespace HyperMsg
{
    public struct DeserializationResult<T> : IEquatable<DeserializationResult<T>>
    {
        public DeserializationResult(int bytesConsumed, T message)
        {
            if (bytesConsumed < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytesConsumed));
            }

            BytesConsumed = bytesConsumed;
            Message = message;
        }

        public int BytesConsumed { get; }

        public T Message { get; }

        public override int GetHashCode() => BytesConsumed.GetHashCode() ^ Message.GetHashCode();

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

            return BytesConsumed.Equals(other.BytesConsumed);
        }
    }
}
