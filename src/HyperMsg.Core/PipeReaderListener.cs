using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class PipeReaderListener : BackgroundWorker
    {
        private readonly PipeReader pipeReader;
        private readonly Func<ReadOnlySequence<byte>, int> bufferReader;

        public PipeReaderListener(PipeReader pipeReader, Func<ReadOnlySequence<byte>, int> bufferReader)
        {
            this.pipeReader = pipeReader ?? throw new ArgumentNullException(nameof(pipeReader));
            this.bufferReader = bufferReader ?? throw new ArgumentNullException(nameof(bufferReader));
        }
        
        protected override async Task DoWorkAsync(CancellationToken token)
        {
            while(!token.IsCancellationRequested)
            {
                var result = await pipeReader.ReadAsync(token);

				if (result.IsCompleted)
				{
					return;
				}

				var bytesReaded = bufferReader(result.Buffer);
				OnBufferReaded();

				if (bytesReaded == 0)
				{
					continue;
				}

	            var position = result.Buffer.GetPosition(bytesReaded);
	            pipeReader.AdvanceTo(position);
			}
        }

	    private void OnBufferReaded()
	    {
		    BufferReaded?.Invoke(this, EventArgs.Empty);
	    }

	    public event EventHandler BufferReaded;
	}
}
