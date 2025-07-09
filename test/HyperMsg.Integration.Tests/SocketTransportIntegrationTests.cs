using HyperMsg.Transport;
using HyperMsg.Transport.Sockets;
using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Integration.Tests;

public class SocketTransportIntegrationTests : IntegrationTestsBase
{
    private static IPEndPoint EndPoint => new(IPAddress.Loopback, 8088);

    private TcpListener _listener = new(EndPoint);

    public SocketTransportIntegrationTests() : base((_, services) => services.AddClientSocketTransport(EndPoint))
    {
    }

    [Fact]
    public void SocketTransport_Should_Resolve()
    {
        // Arrange & Act
        var transport = GetRequiredService<ITransportContext>();
        // Assert
        Assert.NotNull(transport);
        Assert.IsType<SocketTransport>(transport);
    }

    [Fact]
    public async Task SocketTransport_Connection_OpenAsync_Should_Connect()
    {
        // Arrange
        var transport = GetRequiredService<ITransportContext>();
        _listener.Start();
        // Act
        await transport.Connection.OpenAsync(default);
        var acceptedSocket = await _listener.AcceptTcpClientAsync();
        // Assert
        Assert.NotNull(acceptedSocket);
        Assert.Equal(ConnectionState.Connected, transport.Connection.State);
    }

    public override void Dispose()
    {
        base.Dispose();
        _listener.Stop();
    }
}
