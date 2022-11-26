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
    }

    [Fact]
    public void MessageEncoded_Event_Dispatches_Send_Message()
    {
        var bufferContent = Guid.NewGuid().ToByteArray();
        var dispatchedMessage = default(Send);
        messageBroker.Register<Send>(s => dispatchedMessage = s);
        A.CallTo(() => coderGateway.EncodingBuffer.Reader.GetMemory()).Returns(bufferContent);

        coderGateway.MessageEncoded += Raise.FreeForm.With();

        Assert.Equal(bufferContent, dispatchedMessage.Buffer);
    }
}
