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

        public Task ProcessCommandAsync(TransportCommand command, CancellationToken token = default)
        {
            switch (command)
            {
                case TransportCommand.Open:
                    return socket.ConnectAsync(token);

                case TransportCommand.Close:
                    return socket.DisconnectAsync(token);

                case TransportCommand.SetTransportLevelSecurity:
                    SetTls();
                    break;
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
    }
}
