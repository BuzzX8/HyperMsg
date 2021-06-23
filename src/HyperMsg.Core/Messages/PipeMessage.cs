namespace HyperMsg.Messages
{
    internal struct PipeMessage<T>
    {
        public PipeMessage(object pipeId, object portId, T message, IMessageSender messageSender) => 
            (PipeId, PortId, Message, MessageSender) = (pipeId, portId, message, messageSender);

        public object PipeId { get; }

        public object PortId { get; }

        public T Message { get; }

        public IMessageSender MessageSender { get; }
    }
}
