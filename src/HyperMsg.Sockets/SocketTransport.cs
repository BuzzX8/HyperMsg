using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class SocketTransport : IStream, IHandler<TransportOperations>
    {
        private readonly ISocket socket;

        public SocketTransport(ISocket socket)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        public void Handle(TransportOperations command)
        {
            switch (command)
            {
                case TransportOperations.OpenConnection:
                    socket.Connect();
                    break;

                case TransportOperations.CloseConnection:
                    socket.Disconnect();
                    break;

                case TransportOperations.SetTransportLevelSecurity:
                    SetTls();
                    break;
            }
        }

        public Task HandleAsync(TransportOperations command, CancellationToken token = default)
        {
            switch (command)
            {
                case TransportOperations.OpenConnection:
                    return socket.ConnectAsync(token);

                case TransportOperations.CloseConnection:
                    return socket.DisconnectAsync(token);

                case TransportOperations.SetTransportLevelSecurity:
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
