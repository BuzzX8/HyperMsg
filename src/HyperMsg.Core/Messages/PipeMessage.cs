namespace HyperMsg.Messages
{
    internal class PipeMessage<T>
    {
        public PipeMessage(object pipeId, object portId, T message) => 
            (PipeId, PortId, Message) = (pipeId, portId, message);

        public object PipeId { get; }

        public object PortId { get; }

        public T Message { get; }
    }
}
