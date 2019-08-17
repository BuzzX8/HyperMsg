using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class SocketTransport : ITransport
    {
        private readonly ISocket socket;

        public SocketTransport(ISocket socket)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

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

        public async Task HandleFlushRequestAsync(IBufferReader<byte> bufferReader, CancellationToken cancellationToken)
        {
            var ros = bufferReader.Read();
            var enumerator = ros.GetEnumerator();

            while (enumerator.MoveNext())
            {
                await socket.WriteAsync(enumerator.Current, cancellationToken);
            }

            bufferReader.Advance((int)ros.Length);
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

        public event AsyncAction<TransportEventArgs> TransportEvent;
    }
}
