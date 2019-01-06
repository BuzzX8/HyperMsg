using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Transciever
{
	public class BackgroundWorker : IDisposable
	{
		private readonly CancellationTokenSource tokenSource;
        private readonly Func<CancellationToken, Task> workItem;
		private Task listeningTask;

        public BackgroundWorker(Func<CancellationToken, Task> workItem)
        {
            this.workItem = workItem ?? throw new ArgumentNullException(nameof(workItem));
            tokenSource = new CancellationTokenSource();
        }

		public IDisposable Run()
		{
			listeningTask = Task.Run(() => DoWorkAsync(tokenSource.Token))
				.ContinueWith(ListeningTaskContinuation);
			OnStarted();
            return this;
		}

		private void Stop()
		{
			tokenSource.Cancel();
			listeningTask = null;
			OnStopped();
		}

        public void Dispose() => Stop();

        private async Task DoWorkAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await workItem.Invoke(token);
            }
        }

		private void ListeningTaskContinuation(Task task)
		{
			if (task.Status == TaskStatus.Faulted)
			{
				OnError(task.Exception);
				Stop();
			}

			if (task.Status == TaskStatus.RanToCompletion)
			{
				OnCompleted();
			}
		}

		private void OnStarted() => Started?.Invoke(this, EventArgs.Empty);

        protected virtual void OnStopped() => Stopped?.Invoke(this, EventArgs.Empty);

        protected virtual void OnCompleted()
		{
			Stop();
			Completed?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnError(Exception exception) => Error?.Invoke(this, EventArgs.Empty);

		public event EventHandler Started;
		public event EventHandler Stopped;
		public event EventHandler Completed;
		public event EventHandler Error;
	}
}