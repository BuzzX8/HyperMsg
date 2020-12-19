namespace HyperMsg
{
    /// <summary>
    /// Represents message that should be transmitted via transport.
    /// </summary>
    /// <typeparam name="T">Type of message.</typeparam>
    public struct Transmit<T>
    {
        public Transmit(T message) => Message = message;

        /// <summary>
        /// Original message.
        /// </summary>
        public T Message { get; }

        public static implicit operator T(Transmit<T> transmit) => transmit.Message;

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Transmit<T>))
            {
                return false;
            }

            return Equals((Transmit<T>)obj);
        }

        public bool Equals(Transmit<T> other) => Message.Equals(other.Message);

        public override int GetHashCode() => Message.GetHashCode();

        public override string ToString() => $"Transmit({Message})";

        public static bool operator ==(Transmit<T> left, Transmit<T> right) => left.Equals(right);

        public static bool operator !=(Transmit<T> left, Transmit<T> right) => !(left == right);
    }
}