using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class SocketTransport : IStream, IHandler<TransportMessage>
    {
        private readonly ISocket socket;

        public SocketTransport(ISocket socket)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        public void Handle(TransportMessage message)
        {
            switch (message)
            {
                case TransportMessage.Open:
                    socket.Connect();
                    break;

                case TransportMessage.Close:
                    socket.Disconnect();
                    break;

                case TransportMessage.SetTransportLevelSecurity:
                    SetTls();
                    break;
            }
        }

        public Task HandleAsync(TransportMessage message, CancellationToken token = default)
        {
            switch (message)
            {
                case TransportMessage.Open:
                    return socket.ConnectAsync(token);

                case TransportMessage.Close:
                    return socket.DisconnectAsync(token);

                case TransportMessage.SetTransportLevelSecurity:
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
