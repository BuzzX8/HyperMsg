namespace HyperMsg.Transport;

public interface ITransport : IAsyncDisposable
{
    /// <summary>
    /// begins receiving and pushing data to InputBuffer
    /// <param name="cancellationToken">Cancellation token to cancel the operation.</param>
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken);
        
    event Action<Exception> OnError;
    event Action OnDisconnected;
}