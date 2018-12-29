using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class SocketPipe
    {
        private readonly IPipe pipe;
        private readonly ISocket socket;

        private ReadPipeAction readPipe;

        public SocketPipe(IPipe pipe, ISocket socket)
        {
            this.pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            readPipe = new ReadPipeAction(pipe.Reader, Write);
        }

        public async Task TransferFromSocketAsync(CancellationToken token)
        {
            var writer = pipe.Writer;
            var buffer = writer.GetMemory();
            var readed = await socket.ReadAsync(buffer, token);
            writer.Advance(readed);
        }

        public Task TransferToSocketAsync(CancellationToken token) => readPipe.InvokeAsync(token);

        private int Write(ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsSingleSegment)
            {
                socket.Write(buffer.First);
                return buffer.First.Length;
            }

            var enumerator = buffer.GetEnumerator();

            while (enumerator.MoveNext())
            {
                socket.Write(enumerator.Current);
            }

            return (int)buffer.Length;
        }
    }
}
