namespace HyperMsg;

public class Kernel : IDispatcher, IRegistry, ITransportGateway
{
    private readonly Decoder decoder;
    private readonly IEncoder encoder;
    private readonly IBuffer buffer;

    private readonly MessageBroker broker;

    public Kernel(Decoder decoder, IEncoder encoder, IBuffer buffer)
    {
        this.decoder = decoder;
        this.encoder = encoder;
        this.buffer = buffer;
        broker = new();
    }

    public void Dispatch<T>(T message)
    {
        encoder.Encode(buffer.Writer, message);
        MessageSerialized?.Invoke(buffer.Reader);
    }

    public void Register<T>(Action<T> handler) => broker.Register(handler);

    public void Unregister<T>(Action<T> handler) => broker.Unregister(handler);

    public void ReadBuffer(IBufferReader bufferReader) => decoder.Invoke(bufferReader, broker);

    public event Action<IBufferReader> MessageSerialized;
}
