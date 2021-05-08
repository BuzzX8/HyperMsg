namespace HyperMsg.Messages
{
    internal struct ReadFromBufferCommand
    {
        public ReadFromBufferCommand(BufferType bufferType, BufferReader bufferReader) => (BufferType, BufferReader) = (bufferType, bufferReader);

        public BufferType BufferType { get; }

        public BufferReader BufferReader { get; }
    }
}