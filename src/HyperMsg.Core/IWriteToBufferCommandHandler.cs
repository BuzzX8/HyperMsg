namespace HyperMsg
{
    public interface IWriteToBufferCommandHandler
    {
        public void WriteToBuffer<T>(BufferType bufferType, T message);
    }

    public enum BufferType
    {
        None,
        Transmitting,
        Receiving
    }
}
