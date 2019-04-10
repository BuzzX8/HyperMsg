using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class SocketStream : IStream
    {
        private readonly ISocket socket;

        public SocketStream(ISocket socket)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
        }

        public int Read(Memory<byte> buffer) => socket.Read(buffer);

        public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken token = default) => socket.ReadAsync(buffer, token);

        public void Write(Memory<byte> buffer) => socket.Write(buffer);

        public Task WriteAsync(Memory<byte> buffer, CancellationToken token = default) => socket.WriteAsync(buffer, token);
    }
}
