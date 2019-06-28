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
            await OnTransportEventAsync(HyperMsg.TransportEvent.Opening, cancellationToken);
            await socket.ConnectAsync(cancellationToken);
            await OnTransportEventAsync(HyperMsg.TransportEvent.Opened, cancellationToken);
        }

        private async Task CloseAsync(CancellationToken cancellationToken)
        {
            await OnTransportEventAsync(HyperMsg.TransportEvent.Closing, cancellationToken);
            await socket.DisconnectAsync(cancellationToken);
            await OnTransportEventAsync(HyperMsg.TransportEvent.Closed, cancellationToken);
        }

        private Task OnTransportEventAsync(TransportEvent @event, CancellationToken cancellationToken)
        {
            if (TransportEvent != null)
            {
                return TransportEvent.Invoke(new TransportEventArgs(@event), cancellationToken);
            }

            return Task.CompletedTask;
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

        public event AsyncHandler<TransportEventArgs> TransportEvent;
    }
}
