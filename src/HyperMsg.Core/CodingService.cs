namespace HyperMsg;

public class CodingService : ITopic, ICoderGateway, IDisposable
{
    private readonly Decoder decoder;
    private readonly IEncoder encoder;

    private readonly Buffer encodingBuffer;
    private readonly Buffer decodingBuffer;
    private readonly MessageBroker broker;

    public CodingService(Decoder decoder, IEncoder encoder, int decodingBufferSize, int encodingBufferSize)
    {
        this.decoder = decoder;
        this.encoder = encoder;

        decodingBuffer = BufferFactory.Shared.CreateBuffer(decodingBufferSize);
        encodingBuffer = BufferFactory.Shared.CreateBuffer(encodingBufferSize);
        broker = new();
    }

    public IBuffer DecodingBuffer => decodingBuffer;

    public IBuffer EncodingBuffer => encodingBuffer;

    public void Dispatch<T>(T message)
    {
        encoder.Encode(encodingBuffer.Writer, message);
        MessageEncoded?.Invoke();
    }

    public void Register<T>(Action<T> handler) => broker.Register(handler);

    public void Unregister<T>(Action<T> handler) => broker.Unregister(handler);

    public void DecodeMessage() => decoder.Invoke(decodingBuffer.Reader, broker);

    public event Action MessageEncoded;

    public void Dispose()
    {
        encodingBuffer.Dispose();
        decodingBuffer.Dispose();
    }
}
