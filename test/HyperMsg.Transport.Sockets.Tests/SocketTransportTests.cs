using System.Net;
using System.Net.Sockets;
using System.Threading;
using Xunit;

namespace HyperMsg.Transport.Sockets.Tests;

public class SocketTransportTests : IDisposable
{
    private readonly IPEndPoint _endPoint = new IPEndPoint(IPAddress.Loopback, 12345);
    private readonly Socket _socket;
    private readonly TcpListener _listener;
    private readonly SocketTransport _transport;

    public SocketTransportTests()
    {
        _socket = new(SocketType.Stream, ProtocolType.Tcp);
        _listener = new(_endPoint);
        _transport = new(_socket, _endPoint);
    }

    [Fact]
    public async Task OpenAsync_ShouldChangeStateToConnected_WhenSocketIsConnected()
    {
        _listener.Start();
        Assert.Equal(ConnectionState.Disconnected, _transport.State);
        await _transport.OpenAsync(CancellationToken.None);
        Assert.Equal(ConnectionState.Connected, _transport.State);
    }

    [Fact]
    public async Task OpenAsync_ShouldThrow_WhenAlreadyOpen()
    {
        _listener.Start();
        await _transport.OpenAsync(CancellationToken.None);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _transport.OpenAsync(CancellationToken.None));
    }

    [Fact]
    public async Task SendAsync_ShouldThrow_WhenNotConnected()
    {
        await Assert.ThrowsAsync<InvalidOperationException>(() => _transport.SendAsync(new byte[1], CancellationToken.None));
    }

    [Fact]
    public async Task CloseAsync_ShouldBeIdempotent()
    {
        _listener.Start();
        await _transport.OpenAsync(CancellationToken.None);
        await _transport.CloseAsync(CancellationToken.None);
        var stateAfterFirst = _transport.State;
        await _transport.CloseAsync(CancellationToken.None);
        Assert.Equal(stateAfterFirst, _transport.State);
    }

    [Fact]
    public void Constructor_ShouldThrow_OnNullSocket()
    {
        Assert.Throws<ArgumentNullException>(() => new SocketTransport(null!, _endPoint));
    }

    [Fact]
    public void Constructor_ShouldThrow_OnNullEndPoint()
    {
        Assert.Throws<ArgumentNullException>(() => new SocketTransport(_socket, null!));
    }

    [Fact]
    public async Task StateChanged_Event_ShouldFire_OnStateChange()
    {
        _listener.Start();
        ConnectionState? observed = null;
        _transport.StateChanged += s => observed = s;
        await _transport.OpenAsync(CancellationToken.None);
        Assert.Equal(ConnectionState.Connected, observed);
    }

    //[Fact]
    //public async Task OnError_Event_ShouldFire_OnSocketError()
    //{
    //    Exception? observed = null;
    //    _transport.OnError += ex => observed = ex;
    //    // Force error by closing socket before open
    //    _socket.Close();
    //    await Assert.ThrowsAsync<Exception>(() => _transport.OpenAsync(CancellationToken.None));
    //    Assert.NotNull(observed);
    //}

    [Fact]
    public async Task DisposeAsync_ShouldCleanupResources()
    {
        _listener.Start();
        await _transport.OpenAsync(CancellationToken.None);
        await _transport.DisposeAsync();
        Assert.False(_socket.Connected);
    }

    public void Dispose()
    {
        if (_socket.Connected)
        {
            _socket.Shutdown(SocketShutdown.Both);
            _socket.Close();
        }
        _transport.DisposeAsync().AsTask().Wait();
        _listener.Stop();
    }
}
