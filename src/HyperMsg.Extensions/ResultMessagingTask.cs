using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessagingTask<T> : MessagingTaskBase
    {
        private readonly TaskCompletionSource<T> tsc;

        public MessagingTask(IMessagingContext messagingContext, CancellationToken cancellationToken = default) : base(messagingContext, cancellationToken)
        {
            tsc = new TaskCompletionSource<T>();
        }

        public Task<T> Completion => tsc.Task;

        public TaskAwaiter<T> GetAwaiter() => Completion.GetAwaiter();

        protected void SetResult(T result) => tsc.SetResult(result);

        protected void SetCanceled() => tsc.SetCanceled();

        protected void SetException(Exception exception) => tsc.SetException(exception);
    }
}
