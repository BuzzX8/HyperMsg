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
    }
}