namespace HyperMsg.Messages
{
    internal struct ReceiveEvent<T>
    {
        internal ReceiveEvent(T message) => Message = message;

        internal T Message { get; }
    }
}
