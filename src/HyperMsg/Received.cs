namespace HyperMsg
{
    /// <summary>
    /// Decorator that represents message received from transport layer.
    /// </summary>
    /// <typeparam name="T">Message type.</typeparam>
    public struct Received<T>
    {
        public Received(T message) => Message = message;

        /// <summary>
        /// Original message.
        /// </summary>
        public T Message { get; }

        public static implicit operator T(Received<T> received) => received.Message;
    }
}
