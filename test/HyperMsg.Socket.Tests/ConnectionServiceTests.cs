using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Socket.Tests;

public class ConnectionServiceTests : IDisposable
{
    private static readonly IPEndPoint endPoint = new(IPAddress.Loopback, 8080);

    private readonly MessageBroker broker;
    private readonly ConnectionService connectionService;
    private readonly System.Net.Sockets.Socket socket;

    private readonly System.Net.Sockets.Socket acceptingSocket;

    public ConnectionServiceTests()
    {
        broker = new();
        socket = new(SocketType.Stream, ProtocolType.Tcp);
        connectionService = new(broker, broker, socket);

        acceptingSocket = new(SocketType.Stream, ProtocolType.Tcp);
    }

    [Fact]
    public void Dispatching_Connect_Message_Creates_Socket_Connection()
    {
        var result = default(ConnectResult);
        var @event = new ManualResetEventSlim();
        broker.Register<ConnectResult>(c =>
        {
            result = c;
            @event.Set();
        });

        acceptingSocket.Bind(endPoint);
        acceptingSocket.Listen();
        var acceptingTask = acceptingSocket.AcceptAsync();

        broker.Dispatch(new Connect(endPoint));
        @event.Wait(TimeSpan.FromSeconds(2));
        Assert.Equal(endPoint, result.EndPoint);
        Assert.Equal(SocketError.Success, result.Error);
        Assert.True(acceptingTask.IsCompleted);
    }

    public void Dispose()
    {
        connectionService.Dispose();
        acceptingSocket.Close();
        acceptingSocket.Dispose();
    }
}