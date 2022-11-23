using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Socket.Tests;

public class ConnectionServiceTests
{
    private static readonly IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 8080);

    private readonly MessageBroker broker;
    private readonly ConnectionService connectionService;
    private readonly System.Net.Sockets.Socket socket;

    public ConnectionServiceTests()
    {
        broker = new();
        socket = new(SocketType.Stream, ProtocolType.Tcp);
        connectionService = new(broker, broker, socket);
    }

    [Fact]
    public void Dispatching_Connect_Message()
    {
        var result = default(ConnectResult);
        var @event = new ManualResetEventSlim();
        broker.Register<ConnectResult>(c =>
        {
            result = c;
            @event.Set();
        });

        broker.Dispatch(new Connect(endPoint));
        @event.Wait(TimeSpan.FromSeconds(10));

        Assert.Equal(endPoint, result.EndPoint);
        Assert.Equal(SocketError.Success, result.Error);
    }
}