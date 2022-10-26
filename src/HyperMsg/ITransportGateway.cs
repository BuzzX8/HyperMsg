namespace HyperMsg;

public interface ITransportGateway
{
    void ReadBuffer(IBufferReader bufferReader);

    event Action<IBufferReader> MessageSerialized;
}
