using FakeItEasy;

namespace HyperMsg.Socket.Tests;

public class SocketServiceTests
{
    private readonly MessageBroker messageBroker;
    private readonly ICoderGateway coderGateway;
    private readonly SocketService socketService;

    public SocketServiceTests()
    {
        messageBroker = new();
        coderGateway = A.Fake<ICoderGateway>();
        socketService = new(messageBroker, coderGateway);
        socketService.StartAsync(default);
    }

    [Fact]
    public void DispatchReceiveInBufferRequest_Dispatches_ReceiveRequest_Message()
    {
        var bufferContent = Guid.NewGuid().ToByteArray();
        var dispatchedMessage = default(ReceiveRequest);
        messageBroker.Register<ReceiveRequest>(s => dispatchedMessage = s);
        A.CallTo(() => coderGateway.DecodingBuffer.Writer.GetMemory(0)).Returns(bufferContent);

        messageBroker.DispatchReceiveInBufferRequest();

        Assert.Equal(bufferContent, dispatchedMessage.Buffer);
    }

    [Fact]
    public void MessageEncoded_Event_Dispatches_Send_Message()
    {
        var bufferContent = Guid.NewGuid().ToByteArray();
        var dispatchedMessage = default(SendRequest);
        messageBroker.Register<SendRequest>(s => dispatchedMessage = s);
        A.CallTo(() => coderGateway.EncodingBuffer.Reader.GetMemory()).Returns(bufferContent);

        coderGateway.MessageEncoded += Raise.FreeForm.With();

        Assert.Equal(bufferContent, dispatchedMessage.Buffer);
    }

    [Fact]
    public void Dispatching_SendResult_Advances_EncodingBuffer_Reader()
    {
        var bytesTransferred = Guid.NewGuid().ToByteArray()[0];

        messageBroker.Dispatch(new SendResult(bytesTransferred, System.Net.Sockets.SocketError.Success));

        A.CallTo(() => coderGateway.EncodingBuffer.Reader.Advance(bytesTransferred));
    }

    [Fact]
    public void Dispatching_ReceiveResult_Advances_DecodingBuffer_Writer()
    {
        var bytesTransferred = Guid.NewGuid().ToByteArray()[0];

        messageBroker.Dispatch(new ReceiveResult(bytesTransferred, System.Net.Sockets.SocketError.Success));

        A.CallTo(() => coderGateway.DecodingBuffer.Writer.Advance(bytesTransferred));
    }
}
