namespace HyperMsg
{
    internal struct Transmit<T>
    {
        public Transmit(T message) => Message = message;

        internal T Message { get; }
    }
}