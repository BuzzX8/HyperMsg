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
        Dispatch(new Send(memory));
    }

    public void Receive()
    {
        var memory = EncodingBuffer.Writer.GetMemory();
        Dispatch(new Receive(memory));
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
    }

    protected override void UnregisterHandlers(IRegistry registry)
    {
        registry.Unregister<SendResult>(OnSendResult);
        registry.Unregister<ReceiveResult>(OnReceiveResult);
    }

    public override void Dispose()
    {
        base.Dispose();
        coderGateway.MessageEncoded -= MessageEncoded;
    }
}
