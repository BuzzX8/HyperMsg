namespace HyperMsg;

public interface IBufferMediator
{
    void OnReceivingBufferUpdated(IBuffer buffer);

    event Action<IBufferReader> SendingBufferUpdated;
}
