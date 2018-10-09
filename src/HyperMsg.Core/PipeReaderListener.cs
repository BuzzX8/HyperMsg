using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class PipeReaderListener
    {
        private readonly PipeReader pipeReader;
        private readonly Func<ReadOnlySequence<byte>, int> byteReader;

        private CancellationTokenSource tokenSource;
        private Task listeningTask;

        public PipeReaderListener(PipeReader pipeReader, Func<ReadOnlySequence<byte>, int> byteReader)
        {
            this.pipeReader = pipeReader ?? throw new ArgumentNullException(nameof(pipeReader));
            this.byteReader = byteReader ?? throw new ArgumentNullException(nameof(byteReader));
            tokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            listeningTask = Task.Run(ListenReader);
        }

        public void Stop()
        {

        }
        
        private async Task ListenReader()
        {
            var token = tokenSource.Token;

            while(!token.IsCancellationRequested)
            {
                var result = await pipeReader.ReadAsync(token);
                byteReader(result.Buffer);
            }
        }
    }
}
