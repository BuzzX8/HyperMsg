using System;

namespace HyperMsg
{
    /// <summary>
    /// Decorator that represents message received from transport layer.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    public struct Received<T> : IEquatable<Received<T>>
    {
        public Received(T message) => Message = message;

        /// <summary>
        /// Original message.
        /// </summary>
        public T Message { get; }

        public static implicit operator T(Received<T> received) => received.Message;

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Received<T>))
            {
                return false;
            }

            return Equals((Received<T>)obj);
        }

        public bool Equals(Received<T> other) => Message.Equals(other);

        public override int GetHashCode() => Message.GetHashCode();

        public override string ToString() => $"Received({Message})";

        public static bool operator ==(Received<T> left, Received<T> right) => left.Equals(right);

        public static bool operator !=(Received<T> left, Received<T> right) => !(left == right);
    }
}
