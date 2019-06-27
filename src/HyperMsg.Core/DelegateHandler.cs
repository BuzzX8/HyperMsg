using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class DelegateHandler<T> : IMessageHandler<T>
    {
        private readonly Action<T> handleAction;
        private readonly Func<T, CancellationToken, Task<T>> handleAsyncFunc;

        public DelegateHandler(Action<T> handleAction, Func<T, CancellationToken, Task<T>> handleAsyncFunc = null)
        {
            this.handleAction = handleAction ?? throw new ArgumentNullException(nameof(handleAction));
            this.handleAsyncFunc = handleAsyncFunc;
        }

        public Task HandleAsync(T message, CancellationToken cancellationToken = default)
        {
            if (handleAsyncFunc != null)
            {
                return handleAsyncFunc.Invoke(message, cancellationToken);
            }

            handleAction.Invoke(message);
            return Task.CompletedTask;
        }
    }
}
