namespace HyperMsg;

public interface ITransportContext
{
    void OnReceivingBufferUpdated(IBuffer buffer);

    event Action<IBufferReader> SendingBufferUpdated;
}
