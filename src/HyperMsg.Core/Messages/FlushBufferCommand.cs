namespace HyperMsg.Messages
{
    internal struct FlushBufferCommand
    {
        public FlushBufferCommand(BufferType bufferType) => BufferType = bufferType;

        public BufferType BufferType { get; }
    }
}