using System.Net.Sockets;

namespace HyperMsg.Transport.Sockets.Tests;

public class SocketTransportTests : IDisposable
{
    private readonly SocketTransport _transport;
    private readonly Socket _socket;

    private readonly TcpListener _listener;

    public SocketTransportTests()
    {
        _socket = new(SocketType.Stream, ProtocolType.Tcp);
        _transport = new(_socket);
        _listener = new(System.Net.IPAddress.Loopback, 12345);
    }

    [Fact]
    public async Task OpenAsync_ShouldChangeStateToConnected_WhenSocketIsConnected()
    {
        // Arrange
        _listener.Start();
        
        // Act
        await _transport.OpenAsync(CancellationToken.None);
        // Assert
        Assert.Equal(ConnectionState.Connected, _transport.State);
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
