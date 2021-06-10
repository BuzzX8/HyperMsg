namespace HyperMsg.Messages
{
    internal struct FlushBufferEvent
    {
        public FlushBufferEvent(BufferType bufferType, IBufferReader bufferReader) => 
            (BufferType, BufferReader) = (bufferType, bufferReader);

        public BufferType BufferType { get; }

        public IBufferReader BufferReader { get; }
    }
}