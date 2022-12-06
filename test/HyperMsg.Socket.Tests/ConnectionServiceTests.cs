using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Socket.Tests;

public class ConnectionServiceTests : IDisposable
{
    private static readonly IPEndPoint endPoint = new(IPAddress.Loopback, 8080);

    private readonly MessageBroker broker;
    private readonly SocketHolder socketHolder;
    private readonly ConnectionService connectionService;

    private readonly System.Net.Sockets.Socket acceptingSocket;
    private readonly ManualResetEventSlim syncEvent;

    public ConnectionServiceTests()
    {
        broker = new();
        socketHolder = new();
        connectionService = new(broker, socketHolder);
        connectionService.StartAsync(default);

        acceptingSocket = new(SocketType.Stream, ProtocolType.Tcp);
        syncEvent = new();
    }

    #region

    [Fact]
    public void DispatchConnectRequest_Creates_Socket_Connection()
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

        broker.DispatchConnectRequest(endPoint);
        WaitSyncEvent();

        Assert.Equal(endPoint, result.RemoteEndPoint);
        Assert.Equal(SocketError.Success, result.Error);
        acceptingTask.Wait(TimeSpan.FromSeconds(10));
        Assert.True(acceptingTask.IsCompleted);
    }

    [Fact]
    public void DispatchConnectRequest_For_Connected_Socket_Throws_Exception()
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

        broker.DispatchConnectRequest(endPoint);
        WaitSyncEvent();
        syncEvent.Reset();
        
        Assert.Throws<SocketException>(() => broker.DispatchConnectRequest(endPoint));
    }

    #endregion

    #region Disconnect

    [Fact]
    public void DispatchConnectionRequest_Returns_Error_For_Failed_Connection()
    {
        var result = default(ConnectResult);
        
        broker.Register<ConnectResult>(c =>
        {
            result = c;
            SetSyncEvent();
        });

        broker.DispatchConnectRequest(endPoint);
        WaitSyncEvent();

        Assert.Equal(endPoint, result.RemoteEndPoint);
        Assert.Equal(SocketError.ConnectionRefused, result.Error);
    }

    [Fact]
    public void DispatchDisconnectRequest_For_Disonnected_Socket_Throws_Exception()
    {
        var result = default(DisconnectResult);
        
        broker.Register<DisconnectResult>(c =>
        {
            result = c;
            SetSyncEvent();
        });

        broker.DispatchDisconnectRequest();
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
        socketHolder.Dispose();
    }
}