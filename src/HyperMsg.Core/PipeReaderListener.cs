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
        private readonly Func<ReadOnlySequence<byte>, int> bufferReader;

        private CancellationTokenSource tokenSource;
        private Task listeningTask;

        public PipeReaderListener(PipeReader pipeReader, Func<ReadOnlySequence<byte>, int> bufferReader)
        {
            this.pipeReader = pipeReader ?? throw new ArgumentNullException(nameof(pipeReader));
            this.bufferReader = bufferReader ?? throw new ArgumentNullException(nameof(bufferReader));
            tokenSource = new CancellationTokenSource();
        }

		public bool IsListening => listeningTask != null;

        public void Start()
        {
            listeningTask = Task.Run(ListenReader).ContinueWith(t =>
            {
	            OnError(t.Exception);
				Stop();
            }, TaskContinuationOptions.OnlyOnFaulted);
			OnStarted();
        }

        public void Stop()
        {
			tokenSource.Cancel();
			listeningTask = null;
			OnStopped();
        }
        
        private async Task ListenReader()
        {
            var token = tokenSource.Token;

            while(!token.IsCancellationRequested)
            {
                var result = await pipeReader.ReadAsync(token);

				if (result.IsCompleted)
				{
					listeningTask = null;
					OnStopped();
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

		private void OnStarted()
		{
			Started?.Invoke(this, EventArgs.Empty);
		}

		private void OnStopped()
		{
			Stopped?.Invoke(this, EventArgs.Empty);
		}

		private void OnBufferReaded()
		{
			BufferReaded?.Invoke(this, EventArgs.Empty);
		}

	    private void OnError(AggregateException exception)
	    {
			Error?.Invoke(this, EventArgs.Empty);
	    }

		public event EventHandler Started;
		public event EventHandler Stopped;
		public event EventHandler BufferReaded;
	    public event EventHandler Error;
    }
}
