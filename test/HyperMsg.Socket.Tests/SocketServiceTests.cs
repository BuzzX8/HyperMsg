﻿using FakeItEasy;

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
    public void Receive_Dispatches_Receive_Message()
    {
        var bufferContent = Guid.NewGuid().ToByteArray();
        var dispatchedMessage = default(Receive);
        messageBroker.Register<Receive>(s => dispatchedMessage = s);
        A.CallTo(() => coderGateway.EncodingBuffer.Writer.GetMemory(0)).Returns(bufferContent);

        socketService.Receive();
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
