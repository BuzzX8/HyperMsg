
using System.Net.WebSockets;

namespace HyperMsg.Transport;

public class WebSocketTransport(WebSocket webSocket) : ITransport, IConnection, IAsyncDisposable
{
    private readonly WebSocket _webSocket = webSocket;

    public IConnection Connection => throw new NotImplementedException();

    public Stream InputStream => throw new NotImplementedException();

    public Stream OutputStream => throw new NotImplementedException();

    public ConnectionState State => throw new NotImplementedException();

    public event Action<Exception> OnError;
    public event Action OnDisconnected;
    public event Action<int> DataReceived;
    public event Action<int> DataSent;
    public event Action<ConnectionState> ConnectionStateChanged;

    public Task CloseAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public Task OpenAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
