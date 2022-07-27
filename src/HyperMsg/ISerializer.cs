namespace HyperMsg;

public interface ISerializer
{
    void Serialize<T>(IBufferWriter writer, T message);
}
