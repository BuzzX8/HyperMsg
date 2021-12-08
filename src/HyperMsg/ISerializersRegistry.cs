namespace HyperMsg;

public interface ISerializersRegistry
{
    void Add<T>(Action<IBufferWriter, T> serializer);

    Action<IBufferWriter, T> Get<T>();

    bool Contains<T>();
}
