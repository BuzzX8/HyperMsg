using Microsoft.Extensions.Hosting;
using System.Net.Sockets;

namespace HyperMsg.Net;

internal class ClientSocketService : IHostedService, IDisposable
{
    private readonly IRegistry registry;
    private readonly Socket socket;

    public ClientSocketService(IRegistry registry, Socket socket) => (this.registry, this.socket) = (registry, socket);

    public Task StartAsync(CancellationToken cancellationToken)
    {
        registry.Register<BufferUpdatedEvent>(OnTransmitBufferUpdated);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        registry.Deregister<BufferUpdatedEvent>(OnTransmitBufferUpdated);

        return Task.CompletedTask;
    }

    private void OnTransmitBufferUpdated(BufferUpdatedEvent @event)
    {
        var bytes = @event.Buffer.Reader.Read();

        bytes.ForEachSegment(memory => socket.Send(memory.Span));
    }

    public void Dispose() => registry.Deregister<BufferUpdatedEvent>(OnTransmitBufferUpdated);
}
