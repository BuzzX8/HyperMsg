using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class SocketTransport : IStream, IHandler<TransportCommands>
    {
        private readonly ISocket socket;

        public SocketTransport(ISocket socket)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        public void Handle(TransportCommands command)
        {
            switch (command)
            {
                case TransportCommands.OpenConnection:
                    socket.Connect();
                    break;

                case TransportCommands.CloseConnection:
                    socket.Disconnect();
                    break;

                case TransportCommands.SetTransportLevelSecurity:
                    SetTls();
                    break;
            }
        }

        public Task HandleAsync(TransportCommands command, CancellationToken token = default)
        {
            switch (command)
            {
                case TransportCommands.OpenConnection:
                    return socket.ConnectAsync(token);

                case TransportCommands.CloseConnection:
                    return socket.DisconnectAsync(token);

                case TransportCommands.SetTransportLevelSecurity:
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
