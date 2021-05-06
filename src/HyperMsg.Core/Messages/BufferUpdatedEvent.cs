namespace HyperMsg.Messages
{
    internal struct BufferUpdatedEvent
    {
        public BufferUpdatedEvent(BufferType bufferType)
        {
            BufferType = bufferType;
        }

        public BufferType BufferType { get; }
    }
}
