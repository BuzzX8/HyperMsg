namespace HyperMsg;

public interface ICoderGateway
{
    IBuffer DecodingBuffer { get; }

    IBuffer EncodingBuffer { get; }

    void DecodeMessage();

    event Action MessageEncoded;
}
