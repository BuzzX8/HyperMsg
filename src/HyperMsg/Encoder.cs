namespace HyperMsg;

public interface IEncoder
{
    void Encode<T>(IBufferWriter writer, T message);
}

public delegate void Encoder<T>(IBufferWriter writer, T message);