namespace HyperMsg.Transport;

public interface IChannel
{    
    ValueTask<int> SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

    ValueTask<int> ReceiveAsync(Memory<byte> data, CancellationToken cancellationToken = default);
}
