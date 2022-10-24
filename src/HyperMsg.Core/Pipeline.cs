namespace HyperMsg;

public class Pipeline : IDispatcher, IRegistry, IBufferMediator
{
    private readonly Deserializer deserializer;
    private readonly ISerializer serializer;
    private readonly IBuffer buffer;

    private readonly MessageBroker broker;

    public Pipeline(Deserializer deserializer, ISerializer serializer, IBuffer buffer)
    {
        this.deserializer = deserializer;
        this.serializer = serializer;
        this.buffer = buffer;
        broker = new();
    }

    public void Dispatch<T>(T message)
    {
        serializer.Serialize(buffer.Writer, message);
        SendingBufferUpdated?.Invoke(buffer.Reader);
    }

    public void Register<T>(Action<T> handler) => broker.Register(handler);

    public void Deregister<T>(Action<T> handler) => broker.Deregister(handler);

    public void OnReceivingBufferUpdated(IBuffer buffer) => deserializer.Invoke(buffer.Reader, broker);

    public event Action<IBufferReader> SendingBufferUpdated;
}
