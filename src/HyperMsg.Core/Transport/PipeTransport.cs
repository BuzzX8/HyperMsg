namespace HyperMsg.Transport
{
    public class PipeTransport : ITransport
    {
        public event Action<Exception>? OnError;
        public event Action? OnDisconnected;

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
            // Implementation for starting the pipe transport
            return Task.CompletedTask;
        }
    }    
}
