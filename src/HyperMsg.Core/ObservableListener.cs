using System;

namespace HyperMsg
{
	public abstract class ObservableListener<T> : ListenerBase
	{
		private readonly IObserver<T> observer;

		protected ObservableListener(IObserver<T> observer)
		{
			this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
		}

		protected virtual void OnNext(T value)
		{
			observer.OnNext(value);
			Next?.Invoke(this, EventArgs.Empty);
		}

		protected override void OnCompleted()
		{
			base.OnCompleted();
			observer.OnCompleted();
		}

		protected override void OnError(Exception exception)
		{
			base.OnError(exception);
			observer.OnError(exception);
		}

		public event EventHandler Next;
	}
}
