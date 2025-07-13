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
        _listener.Start();
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
        
        // Act
        await transport.Connection.OpenAsync(default);
        var acceptedSocket = await _listener.AcceptTcpClientAsync();

        // Assert
        Assert.NotNull(acceptedSocket);
        Assert.Equal(ConnectionState.Connected, transport.Connection.State);
    }

    [Fact]
    public void SocketTransport_Connection_Should_Return_Valid_Connection()
    {
        var transport = GetRequiredService<ITransportContext>();
        Assert.NotNull(transport.Connection);
        Assert.Equal(ConnectionState.Disconnected, transport.Connection.State);
    }

    [Fact]
    public void SocketTransport_ReceiveDataHandlers_Should_Be_Modifiable()
    {
        var transport = GetRequiredService<ITransportContext>();
        bool handlerCalled = false;
        ReceiveDataHandler handler = (data, ct) => { handlerCalled = true; return ValueTask.CompletedTask; };
        transport.ReceiveDataHandlers.Add(handler);

        // Simulate invocation
        foreach (var h in transport.ReceiveDataHandlers)
            h.Invoke(new ReadOnlyMemory<byte>(new byte[] { 1, 2, 3 }), default).GetAwaiter().GetResult();

        Assert.True(handlerCalled);
    }

    [Fact]
    public async Task SocketTransport_SendAsync_Should_Send_Data_When_Connected()
    {
        var transport = GetRequiredService<ITransportContext>();
        await transport.Connection.OpenAsync(default);
        var acceptedSocket = await _listener.AcceptTcpClientAsync();

        var data = new byte[] { 10, 20, 30 };
        await transport.SendAsync(data, default);

        // Receive data on the server side
        var buffer = new byte[3];
        var stream = acceptedSocket.GetStream();
        int bytesRead = await stream.ReadAsync(buffer);

        Assert.Equal(data.Length, bytesRead);
        Assert.Equal(data, buffer);
    }

    [Fact]
    public async Task SocketTransport_SendAsync_Should_Throw_If_Not_Connected()
    {
        var transport = GetRequiredService<ITransportContext>();
        var data = new byte[] { 1, 2, 3 };
        await Assert.ThrowsAsync<InvalidOperationException>(() => transport.SendAsync(data, default));
    }

    public override void Dispose()
    {
        base.Dispose();
        _listener.Stop();
    }
}
