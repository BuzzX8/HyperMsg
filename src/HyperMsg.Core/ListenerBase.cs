﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
	public abstract class ListenerBase
	{
		private readonly CancellationTokenSource tokenSource = new CancellationTokenSource();
		private Task listeningTask;

		public bool IsListening => listeningTask != null;

		public void Start()
		{
			listeningTask = DoListening(tokenSource.Token)
				.ContinueWith(ListeningTaskContinuation);
			OnStarted();
		}

		public void Stop()
		{
			tokenSource.Cancel();
			listeningTask = null;
			OnStopped();
		}

		protected abstract Task DoListening(CancellationToken token);

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

		private void OnStarted()
		{
			Started?.Invoke(this, EventArgs.Empty);
		}

		private void OnStopped()
		{
			Stopped?.Invoke(this, EventArgs.Empty);
		}

		private void OnCompleted()
		{
			Stop();
		}

		private void OnError(AggregateException exception)
		{
			Error?.Invoke(this, EventArgs.Empty);
		}

		public event EventHandler Started;
		public event EventHandler Stopped;
		public event EventHandler Error;
	}
}
