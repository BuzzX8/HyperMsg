namespace HyperMsg.Messages
{
    internal struct SerializeCommand<T>
    {
        internal SerializeCommand(IBufferWriter bufferWriter, T message)
         => (BufferWriter, Message) = (bufferWriter, message);

        public IBufferWriter BufferWriter { get; }

        public T Message { get; }
    }
}
