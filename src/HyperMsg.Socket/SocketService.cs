namespace HyperMsg.Socket;

public class SocketService : Service
{
    private readonly IBuffer receivingBuffer;
    private readonly ICoderGateway coderGateway;

    private IBufferReader? reader;

    public SocketService(ITopic socketTopic, ICoderGateway coderGateway, IBuffer receivingBuffer) : base(socketTopic)
    {
        this.coderGateway = coderGateway ?? throw new ArgumentNullException(nameof(coderGateway));
        this.receivingBuffer = receivingBuffer ?? throw new ArgumentNullException(nameof(receivingBuffer));
        this.coderGateway.MessageEncoded += MessageEncoded;
    }

    private void MessageEncoded(IBufferReader reader)
    {
        this.reader = reader;
        //Dispatch(new Send(reader.GetMemory()));
    }

    private void Receive()
    {
        var memory = receivingBuffer.Writer.GetMemory();
        Dispatch(new Receive(memory));
    }

    private void OnSendResult(SendResult message)
    {
        if (reader is null)
        {
            return;
        }

        if (message.Error != System.Net.Sockets.SocketError.Success)
        {
            reader.Advance(message.BytesTransferred);
        }

        reader = null;
    }

    private void OnReceiveResult(ReceiveResult message)
    {
        if (message.Error != System.Net.Sockets.SocketError.Success)
        {
            return;
        }

        coderGateway.DecodeMessage(receivingBuffer.Reader);
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
