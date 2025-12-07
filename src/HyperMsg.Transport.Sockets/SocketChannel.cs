namespace HyperMsg.Transport.Sockets;

internal class SocketChannel(ISocket socket) : IChannel
{
    private readonly ISocket _socket = socket;

    public ValueTask<int> SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return _socket.SendAsync(data, cancellationToken);
    }
    public ValueTask<int> ReceiveAsync(Memory<byte> data, CancellationToken cancellationToken = default)
    {
        return _socket.ReceiveAsync(data, cancellationToken);
    }
}
