
using System.Net.WebSockets;

namespace HyperMsg.Transport;

public class WebSocketTransport(WebSocket webSocket) : ITransport
{
    private readonly WebSocket _webSocket = webSocket;

    public event Action<Exception> OnError;
    public event Action OnDisconnected;

    public ValueTask DisposeAsync()
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
