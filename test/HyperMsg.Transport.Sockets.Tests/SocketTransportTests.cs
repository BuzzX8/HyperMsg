using FakeItEasy;
using HyperMsg.Buffers;

namespace HyperMsg.Transport.Sockets.Tests;

public class SocketTransportTests : IDisposable
{
    private readonly ISocket _socket;
    private readonly BufferingContext _bufferingContext = new();
    private readonly SocketTransport _transport;

    public SocketTransportTests()
    {
        _socket = A.Fake<ISocket>();
        _transport = new(_socket, _bufferingContext);
    }

    [Fact]
    public async Task OpenAsync_ShouldChangeStateToConnected_WhenSocketIsConnected()
    {        
        Assert.Equal(ConnectionState.Disconnected, _transport.Connection.State);
        await _transport.Connection.OpenAsync(CancellationToken.None);
        Assert.Equal(ConnectionState.Connected, _transport.Connection.State);
        A.CallTo(() => _socket.OpenAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task OpenAsync_ShouldThrow_WhenAlreadyOpen()
    {
        await _transport.Connection.OpenAsync(CancellationToken.None);
        await Assert.ThrowsAsync<InvalidOperationException>(() => _transport.Connection.OpenAsync(CancellationToken.None).AsTask());
    }

    [Fact]
    public async Task CloseAsync_ShouldBeIdempotent()
    {        
        await _transport.Connection.OpenAsync(CancellationToken.None);
        await _transport.Connection.CloseAsync(CancellationToken.None);
        var stateAfterFirst = _transport.Connection.State;
        await _transport.Connection.CloseAsync(CancellationToken.None);
        Assert.Equal(stateAfterFirst, _transport.Connection.State);
    }

    [Fact]
    public async Task StateChanged_Event_ShouldFire_OnStateChange()
    {        
        ConnectionState? observed = null;
        _transport.Connection.StateChanged += s => observed = s;
        await _transport.Connection.OpenAsync(CancellationToken.None);
        Assert.Equal(ConnectionState.Connected, observed);
    }

    [Fact]
    public async Task DisposeAsync_ShouldCleanupResources()
    {
        await _transport.Connection.OpenAsync(CancellationToken.None);
        _transport.Dispose();
        Assert.Equal(ConnectionState.Disconnected, _transport.Connection.State);
        A.CallTo(() => _socket.CloseAsync(A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task BufferingContext_RequestOutputBufferDownstreamUpdate_()
    {
        var message = Guid.NewGuid().ToByteArray();
        _bufferingContext.OutputBuffer.Writer.Write(message);

        var receivedMessage = default(byte[]);

        A.CallTo(() => _socket.SendAsync(A<ReadOnlyMemory<byte>>.Ignored, A<CancellationToken>.Ignored))
            .ReturnsLazily((ReadOnlyMemory<byte> buffer, CancellationToken _) =>
            {
                receivedMessage = buffer.ToArray();
                return buffer.Length;
            });

        await _bufferingContext.RequestOutputBufferDownstreamUpdate(CancellationToken.None);

        Assert.Equal(message, receivedMessage);

        A.CallTo(() => _socket.SendAsync(A<ReadOnlyMemory<byte>>.Ignored, A<CancellationToken>.Ignored))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task BufferingContext_RequestInputBufferUpstreamUpdate_()
    {
        var dataToReceive = Guid.NewGuid().ToByteArray();

        A.CallTo(() => _socket.ReceiveAsync(A<Memory<byte>>.Ignored, A<CancellationToken>.Ignored))
            .ReturnsLazily((Memory<byte> buffer, CancellationToken _) =>
            {
                dataToReceive.CopyTo(buffer);
                return dataToReceive.Length;
            });

        await _bufferingContext.RequestInputBufferUpstreamUpdate(CancellationToken.None);

        var receivedMessage = _bufferingContext.InputBuffer.Reader.GetMemory();
        
        Assert.Equal(dataToReceive, receivedMessage);
    }

    public void Dispose() => _transport?.Dispose();
}
