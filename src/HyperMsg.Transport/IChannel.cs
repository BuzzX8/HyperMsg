namespace HyperMsg.Transport;

public interface IChannel
{    
    Task<int> SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

    Task<int> ReceiveAsync(Memory<byte> data, CancellationToken cancellationToken = default);
}
