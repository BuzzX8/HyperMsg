namespace HyperMsg
{
    public interface IBufferContext
    {
        IBuffer ReceivingBuffer { get; }

        IBuffer TransmittingBuffer { get; }
    }
}
