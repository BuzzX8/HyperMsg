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
    private readonly ManualResetEventSlim syncEvent;

    public ConnectionServiceTests()
    {
        broker = new();
        socket = new(SocketType.Stream, ProtocolType.Tcp);
        connectionService = new(broker, socket);
        connectionService.StartAsync(default);

        acceptingSocket = new(SocketType.Stream, ProtocolType.Tcp);
        syncEvent = new();
    }

    #region

    [Fact]
    public void Dispatching_Connect_Message_Creates_Socket_Connection()
    {
        var result = default(ConnectResult);
        
        broker.Register<ConnectResult>(c =>
        {
            result = c;
            SetSyncEvent();
        });

        acceptingSocket.Bind(endPoint);
        acceptingSocket.Listen();
        var acceptingTask = acceptingSocket.AcceptAsync();

        broker.Dispatch(new Connect(endPoint));
        WaitSyncEvent();

        Assert.Equal(endPoint, result.RemoteEndPoint);
        Assert.Equal(SocketError.Success, result.Error);
        acceptingTask.Wait(TimeSpan.FromSeconds(10));
        Assert.True(acceptingTask.IsCompleted);
    }

    [Fact]
    public void Dispatching_Connect_Message_For_Connected_Socket()
    {
        var result = default(ConnectResult);

        broker.Register<ConnectResult>(c =>
        {
            result = c;
            SetSyncEvent();
        });

        acceptingSocket.Bind(endPoint);
        acceptingSocket.Listen();
        var acceptingTask = acceptingSocket.AcceptAsync();

        broker.Dispatch(new Connect(endPoint));
        WaitSyncEvent();
        syncEvent.Reset();
        
        Assert.Throws<SocketException>(() => broker.Dispatch(new Connect(endPoint)));
    }

    #endregion

    #region Disconnect

    [Fact]
    public void Dispatching_Connect_Message_Returns_Error()
    {
        var result = default(ConnectResult);
        
        broker.Register<ConnectResult>(c =>
        {
            result = c;
            SetSyncEvent();
        });

        broker.Dispatch(new Connect(endPoint));
        WaitSyncEvent();

        Assert.Equal(endPoint, result.RemoteEndPoint);
        Assert.Equal(SocketError.ConnectionRefused, result.Error);
    }

    [Fact]
    public void Dispatching_Disconnect_Message_For_Disonnected_Socket()
    {
        var result = default(DisconnectResult);
        
        broker.Register<DisconnectResult>(c =>
        {
            result = c;
            SetSyncEvent();
        });

        broker.Dispatch(new Disconnect());
        WaitSyncEvent();

        Assert.Equal(SocketError.NotConnected, result.Error);
    }

    #endregion

    private void SetSyncEvent() => syncEvent.Set();

    private void WaitSyncEvent() => syncEvent.Wait(TimeSpan.FromSeconds(10));

    public void Dispose()
    {
        connectionService.Dispose();
        acceptingSocket.Close();
        acceptingSocket.Dispose();
    }
}