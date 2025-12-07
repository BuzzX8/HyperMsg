namespace HyperMsg.Transport.Sockets;

public interface ISocket
{
    ValueTask OpenAsync(CancellationToken cancellationToken);
        
    ValueTask CloseAsync(CancellationToken cancellationToken);
        
    ValueTask<int> SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken);

    ValueTask<int> ReceiveAsync(Memory<byte> memory, CancellationToken cancellationToken);

    event EventHandler<ReadOnlyMemory<byte>> OnDataSent;

    event EventHandler<Memory<byte>> OnDataReceived;
}
