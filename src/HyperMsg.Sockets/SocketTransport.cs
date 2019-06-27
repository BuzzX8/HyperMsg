using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class SocketTransport : ITransport, IStream
    {
        private readonly ISocket socket;

        public SocketTransport(ISocket socket)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        public IStream GetStream() => this;

        public Task ProcessCommandAsync(TransportCommand command, CancellationToken cancellationToken)
        {
            switch (command)
            {
                case TransportCommand.Open:
                    return OpenAsync(cancellationToken);

                case TransportCommand.Close:
                    return CloseAsync(cancellationToken);

                case TransportCommand.SetTransportLevelSecurity:
                    SetTls();
                    break;
            }

            return Task.CompletedTask;
        }

        private async Task OpenAsync(CancellationToken cancellationToken)
        {
            OnTransportEvent(HyperMsg.TransportEvent.Opening);
            await socket.ConnectAsync(cancellationToken);
            OnTransportEvent(HyperMsg.TransportEvent.Opened);
        }

        private async Task CloseAsync(CancellationToken cancellationToken)
        {
            OnTransportEvent(HyperMsg.TransportEvent.Closing);
            await socket.DisconnectAsync(cancellationToken);
            OnTransportEvent(HyperMsg.TransportEvent.Closed);
        }

        private void OnTransportEvent(TransportEvent @event)
        {
            TransportEvent?.Invoke(this, new TransportEventArgs(@event));
        }

        private void SetTls()
        {
            if (!(socket is ISupportsTls tls))
            {
                throw new NotSupportedException();
            }

            tls.SetTls();
        }

        public int Read(Memory<byte> buffer) => socket.Read(buffer);

        public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token = default) => socket.ReadAsync(buffer, token);

        public void Write(Memory<byte> buffer) => socket.Write(buffer);

        public Task WriteAsync(Memory<byte> buffer, CancellationToken token = default) => socket.WriteAsync(buffer, token);

        public event EventHandler<TransportEventArgs> TransportEvent;
    }
}
