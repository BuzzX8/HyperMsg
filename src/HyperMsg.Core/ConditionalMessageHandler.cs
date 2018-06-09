using System;
using System.Threading.Tasks;

namespace HyperMsg
{
	internal class ConditionalMessageHandler<T>
	{
		private Action<T> handler;
		private Func<T, Task> asyncHandler;
		private Func<T, bool> predicate;

		internal ConditionalMessageHandler(Action<T> handler, Func<T, bool> predicate)
		{
			this.handler = handler;
			this.predicate = predicate;
		}

		internal ConditionalMessageHandler(Func<T, Task> asyncHandler, Func<T, bool> predicate)
		{
			this.asyncHandler = asyncHandler;
			this.predicate = predicate;
		}

		public void Handle(T message)
		{
			if (predicate(message))
			{
				handler(message);
			}
		}

		public Task HandleAsync(T message)
		{
			return !predicate(message) ? Task.CompletedTask : asyncHandler(message);
		}
	}
}