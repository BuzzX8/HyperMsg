namespace HyperMsg
{
    public struct Transmit<T>
    {
        public Transmit(T message) => Message = message;

        public T Message { get; }

        public static implicit operator T(Transmit<T> transmit) => transmit.Message;
    }
}