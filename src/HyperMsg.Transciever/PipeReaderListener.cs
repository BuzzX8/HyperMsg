using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class PipeReaderListener// : BackgroundWorker
    {
        private readonly IPipeReader pipeReader;
        private readonly Func<ReadOnlySequence<byte>, int> bufferReader;

        public PipeReaderListener(IPipeReader pipeReader, Func<ReadOnlySequence<byte>, int> bufferReader)
        {
            this.pipeReader = pipeReader ?? throw new ArgumentNullException(nameof(pipeReader));
            this.bufferReader = bufferReader ?? throw new ArgumentNullException(nameof(bufferReader));
        }
        
        protected async Task DoWorkAsync(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                var result = await pipeReader.ReadAsync(token);

				var bytesReaded = bufferReader(result);
				OnBufferReaded();

				if (bytesReaded == 0)
				{
					continue;
				}

                pipeReader.Advance(bytesReaded);
			}
        }

	    private void OnBufferReaded()
	    {
		    BufferReaded?.Invoke(this, EventArgs.Empty);
	    }

	    public event EventHandler BufferReaded;
	}
}