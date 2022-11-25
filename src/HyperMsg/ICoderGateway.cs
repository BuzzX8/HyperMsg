namespace HyperMsg;

public interface ICoderGateway
{
    void DecodeMessage(IBufferReader bufferReader);

    event Action<IBufferReader> MessageEncoded;
}
