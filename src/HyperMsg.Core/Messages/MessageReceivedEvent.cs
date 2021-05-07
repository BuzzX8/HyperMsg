namespace HyperMsg.Messages
{
    internal struct MessageReceivedEvent<T>
    {
        internal MessageReceivedEvent(T message) => Message = message;

        internal T Message { get; }
    }
}
