namespace HyperMsg
{
    public struct Received<T>
    {
        public Received(T message) => Message = message;

        public T Message { get; }

        public static implicit operator T(Received<T> received) => received.Message;
    }
}
