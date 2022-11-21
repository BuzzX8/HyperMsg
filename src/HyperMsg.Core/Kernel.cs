namespace HyperMsg;

public class Kernel : IDispatcher, IRegistry, ITransportGateway
{
    private readonly Decoder deserializer;
    private readonly IEncoder serializer;
    private readonly IBuffer buffer;

    private readonly MessageBroker broker;

    public Kernel(Decoder deserializer, IEncoder serializer, IBuffer buffer)
    {
        this.deserializer = deserializer;
        this.serializer = serializer;
        this.buffer = buffer;
        broker = new();
    }

    public void Dispatch<T>(T message)
    {
        serializer.Encode(buffer.Writer, message);
        MessageSerialized?.Invoke(buffer.Reader);
    }

    public void Register<T>(Action<T> handler) => broker.Register(handler);

    public void Deregister<T>(Action<T> handler) => broker.Deregister(handler);

    public void ReadBuffer(IBufferReader bufferReader) => deserializer.Invoke(bufferReader, broker);

    public event Action<IBufferReader> MessageSerialized;
}
