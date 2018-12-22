using System;

namespace HyperMsg
{
	public abstract class ObservableWorker<T>// : BackgroundWorker
	{
		private readonly IObserver<T> observer;

		protected ObservableWorker(IObserver<T> observer)
		{
			this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
		}

		protected virtual void OnNext(T value)
		{
			observer.OnNext(value);
			Next?.Invoke(this, EventArgs.Empty);
		}

		protected void OnCompleted()
		{
			//base.OnCompleted();
			observer.OnCompleted();
		}

		protected void OnError(Exception exception)
		{
			//base.OnError(exception);
			observer.OnError(exception);
		}

		public event EventHandler Next;
	}
}
