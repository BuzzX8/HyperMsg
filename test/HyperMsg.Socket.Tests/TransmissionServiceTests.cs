using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Socket.Tests;

public class TransmissionServiceTests : IDisposable
{
    private static readonly IPEndPoint endPoint = new(IPAddress.Loopback, 8081);

    private readonly MessageBroker broker;
    private readonly SocketHolder socketHolder;
    private readonly TransmissionService transmissionService;

    private readonly System.Net.Sockets.Socket acceptingSocket;
    private readonly ManualResetEventSlim syncEvent;

    public TransmissionServiceTests()
    {
        broker = new();
        socketHolder = new();
        transmissionService = new(broker, socketHolder);
        transmissionService.StartAsync(default);

        acceptingSocket = new(SocketType.Stream, ProtocolType.Tcp);
        syncEvent = new();
    }

    [Fact]
    public void Dispatching_Send_Message_()
    {
        var message = Guid.NewGuid().ToByteArray();
        var acceptedMessage = new byte[message.Length];
        var result = default(SendResult);

        broker.Register<SendResult>(r =>
        {
            result = r;
            SetSyncEvent();
        });

        acceptingSocket.Bind(endPoint);
        acceptingSocket.Listen();

        socketHolder.Socket.Connect(endPoint);
        var acceptedSocket = acceptingSocket.Accept();

        broker.DispatchSendRequest(message);

        WaitSyncEvent();
        var received = acceptedSocket.Receive(acceptedMessage);

        Assert.Equal(message, acceptedMessage);
        Assert.Equal(message.Length, result.BytesTransferred);
        Assert.Equal(SocketError.Success, result.Error);
    }

    [Fact]
    public void Dispatching_Receive_Message_() 
    {
        var message = Guid.NewGuid().ToByteArray();
        var acceptedMessage = new byte[message.Length];
        var result = default(ReceiveResult);

        broker.Register<ReceiveResult>(r =>
        {
            result = r;
            SetSyncEvent();
        });

        acceptingSocket.Bind(endPoint);
        acceptingSocket.Listen();

        socketHolder.Socket.Connect(endPoint);
        var acceptedSocket = acceptingSocket.Accept();

        broker.DispatchReceiveRequest(acceptedMessage);
        acceptedSocket.Send(message);

        WaitSyncEvent();

        Assert.Equal(message, acceptedMessage);
        Assert.Equal(SocketError.Success, result.Error);
        Assert.Equal(message.Length, result.BytesTransferred);
    }

    private void SetSyncEvent() => syncEvent.Set();

    private void WaitSyncEvent() => syncEvent.Wait(TimeSpan.FromSeconds(10));

    public void Dispose()
    {
        socketHolder.Dispose();
        transmissionService.Dispose();
        acceptingSocket.Close();
        acceptingSocket.Dispose();
    }
}
