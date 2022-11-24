namespace HyperMsg;

public interface ICoderGateway
{
    void TryDecodeMessage(IBufferReader bufferReader);

    event Action<IBufferReader> MessageEncoded;
}
