namespace HyperMsg.Messages
{
    internal struct ReadFromBufferCommand
    {
        public ReadFromBufferCommand(BufferType bufferType, BufferReader bufferReader)
        {
            BufferType = bufferType;
            BufferReader = bufferReader;
        }

        public BufferType BufferType { get; }

        public BufferReader BufferReader { get; }
    }
}