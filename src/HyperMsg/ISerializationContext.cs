namespace HyperMsg;

public interface ISerializationContext
{
    ISerializer Serializer { get; set; }

    Action<IBufferReader, IDispatcher> Deserializer { get; set; }
}
