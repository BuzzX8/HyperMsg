namespace HyperMsg.Transport;

public interface ITransportContext : IAsyncDisposable
{
    IConnection Connection { get; }
    
    Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default);

    event Action<ReadOnlyMemory<byte>> DataReceived;
}