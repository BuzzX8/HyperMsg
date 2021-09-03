namespace HyperMsg.Messages
{
    internal struct SerializationCommand<T>
    {
        public ISender Sender { get; }

        public T Message { get; }
    }
}