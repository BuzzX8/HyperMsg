using HyperMsg.Transciever;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Sockets
{
    public class SocketWorker : IDisposable
    {
        private readonly BackgroundWorker readerWorker;
        private readonly BackgroundWorker writerWorker;
        private IDisposable readerRun;
        private IDisposable writerRun;

        private readonly IPipe pipe;
        private readonly ISocket socket;
        private readonly SocketPipe socketPipe;

        public SocketWorker(IPipe pipe, ISocket socket)
        {
            this.pipe = pipe ?? throw new ArgumentNullException(nameof(pipe));
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            socketPipe = new SocketPipe(pipe, socket);

            readerWorker = new BackgroundWorker(Read);
            writerWorker = new BackgroundWorker(Write);
        }

        public IDisposable Run() => this;

        public void Dispose()
        {
            readerRun.Dispose();
            writerRun.Dispose();
        }

        private void RunIfRequired()
        {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            if (readerRun == writerRun == null)
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            {
                readerRun = readerWorker.Run();
                writerRun = writerWorker.Run();
            }
        }

        private Task Read(CancellationToken token) => socketPipe.TransferToSocketAsync(token);

        private Task Write(CancellationToken token) => socketPipe.TransferFromSocketAsync(token);
    }
}
