using HyperMsg.Buffers;
using HyperMsg.Transport;
using HyperMsg.Transport.Sockets;
using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Integration.Tests;

public class SocketTransportIntegrationTests : IntegrationTestsBase
{
    private static IPEndPoint EndPoint => new(IPAddress.Loopback, 8088);

    private TcpListener _listener = new(EndPoint);

    public SocketTransportIntegrationTests() : base((_, services) => services.AddBufferingContext().AddClientSocketTransport(EndPoint))
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

    [Fact]
    public async Task SocketTransport_Connection_CloseAsync_Should_Disconnect()
    {
        // Arrange
        var transport = GetRequiredService<ITransportContext>();
        await transport.Connection.OpenAsync(default);
        var acceptedSocket = await _listener.AcceptTcpClientAsync();
        // Act
        await transport.Connection.CloseAsync(default);
        // Assert
        Assert.Equal(ConnectionState.Disconnected, transport.Connection.State);
    }

    [Fact]
    public async Task SocketTransport_Connection_CloseAsync_Should_Not_Throw_When_Not_Connected()
    {
        // Arrange
        var transport = GetRequiredService<ITransportContext>();
        // Act & Assert
        await transport.Connection.CloseAsync(default);
        Assert.Equal(ConnectionState.Disconnected, transport.Connection.State);
    }

    [Fact]
    public async Task SocketTransport_Interacts_With_BufferingContext()
    {
        // Arrange
        var bufferingContext = GetRequiredService<IBufferingContext>();
        var transport = GetRequiredService<ITransportContext>();
        await transport.Connection.OpenAsync(default);
        var acceptedSocket = await _listener.AcceptTcpClientAsync();
        // Act
        var data = new byte[] { 1, 2, 3 };
        bufferingContext.Output.Writer.Write(data);
        //bufferingContext.Output.Writer.Advance(data.Length);
        await bufferingContext.RequestOutputBufferHandling(default);
        // Assert
        Assert.NotEmpty(bufferingContext.OutputHandlers);
        //Assert.Equal(data, bufferingContext.Output.ToArray());
    }

    public override void Dispose()
    {
        base.Dispose();
        _listener.Stop();
    }
}
