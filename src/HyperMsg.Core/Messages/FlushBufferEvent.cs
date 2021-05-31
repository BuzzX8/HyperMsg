namespace HyperMsg.Messages
{
    internal struct FlushBufferEvent
    {
        public FlushBufferEvent(BufferType bufferType, IBufferReader<byte> bufferReader) => 
            (BufferType, BufferReader) = (bufferType, bufferReader);

        public BufferType BufferType { get; }

        public IBufferReader<byte> BufferReader { get; }
    }
}