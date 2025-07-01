using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Transport.Sockets.Tests;

public class SocketTransportTests : IDisposable
{
    private readonly IPEndPoint _endPoint = new IPEndPoint(IPAddress.Loopback, 12345);
    private readonly SocketTransport _transport;
    private readonly Socket _socket;

    private readonly TcpListener _listener;

    public SocketTransportTests()
    {
        _socket = new(SocketType.Stream, ProtocolType.Tcp);
        _transport = new(_socket, _endPoint);
        _listener = new(_endPoint);
    }

    [Fact]
    public async Task OpenAsync_ShouldChangeStateToConnected_WhenSocketIsConnected()
    {
        // Arrange
        _listener.Start();
        
        // Act
        Assert.Equal(ConnectionState.Disconnected, _transport.State);
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
