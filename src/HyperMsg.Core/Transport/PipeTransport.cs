
namespace HyperMsg.Transport
{
    public class PipeTransport : ITransport
    {
        public IConnection Connection => throw new NotImplementedException();

        public Stream InputStream => throw new NotImplementedException();

        public Stream OutputStream => throw new NotImplementedException();

        public event Action<Exception>? OnError;
        public event Action? OnDisconnected;
        public event Action<int> DataReceived;
        public event Action<int> DataSent;
        public event Action<ConnectionState> ConnectionStateChanged;

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
