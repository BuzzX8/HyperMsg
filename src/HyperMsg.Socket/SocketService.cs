namespace HyperMsg.Socket;

public class SocketService : Service
{    
    private readonly ICoderGateway coderGateway;

    public SocketService(ITopic socketTopic, ICoderGateway coderGateway) : base(socketTopic)
    {
        this.coderGateway = coderGateway ?? throw new ArgumentNullException(nameof(coderGateway));        
        this.coderGateway.MessageEncoded += MessageEncoded;
    }

    private IBuffer DecodingBuffer => coderGateway.DecodingBuffer;

    private IBuffer EncodingBuffer => coderGateway.EncodingBuffer;

    private void MessageEncoded()
    {
        var memory = EncodingBuffer.Reader.GetMemory();
        Dispatch(new SendRequest(memory));
    }

    private void OnReceiveInBuffer(ReceiveInBufferRequest _)
    {
        var memory = DecodingBuffer.Writer.GetMemory();
        Dispatch(new ReceiveRequest(memory));
    }

    private void OnSendResult(SendResult message)
    {
        if (message.Error != System.Net.Sockets.SocketError.Success)
        {
            return;
        }

        EncodingBuffer.Reader.Advance(message.BytesTransferred);
    }

    private void OnReceiveResult(ReceiveResult message)
    {
        if (message.Error != System.Net.Sockets.SocketError.Success)
        {
            return;
        }

        DecodingBuffer.Writer.Advance(message.BytesTransferred);
        coderGateway.DecodeMessage();
    }

    protected override void RegisterHandlers(IRegistry registry)
    {
        registry.Register<SendResult>(OnSendResult);
        registry.Register<ReceiveResult>(OnReceiveResult);
        registry.Register<ReceiveInBufferRequest>(OnReceiveInBuffer);
    }

    protected override void UnregisterHandlers(IRegistry registry)
    {
        registry.Unregister<SendResult>(OnSendResult);
        registry.Unregister<ReceiveResult>(OnReceiveResult);
        registry.Unregister<ReceiveInBufferRequest>(OnReceiveInBuffer);
    }

    public override void Dispose()
    {
        base.Dispose();
        coderGateway.MessageEncoded -= MessageEncoded;
    }
}

public record struct ReceiveInBufferRequest();