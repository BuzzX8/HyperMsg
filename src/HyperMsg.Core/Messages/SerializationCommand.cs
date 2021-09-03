namespace HyperMsg.Messages
{
    internal struct SerializationCommand<T>
    {
        public SerializationCommand(ISender sender, T message) => (Sender, Message) = (sender, message);

        public ISender Sender { get; }

        public T Message { get; }
    }
}