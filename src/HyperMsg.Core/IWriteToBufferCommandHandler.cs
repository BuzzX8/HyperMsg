namespace HyperMsg
{
    public interface IWriteToBufferCommandHandler
    {
        public void WriteToBuffer<T>(BufferType bufferType, T message, bool flushBuffer);
    }

    public enum BufferType
    {
        None,
        Transmitting,
        Receiving
    }
}
