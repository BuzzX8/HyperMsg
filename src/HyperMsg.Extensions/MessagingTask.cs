using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessagingTask : MessagingTaskBase
    {
        private readonly TaskCompletionSource<bool> tsc;

        public MessagingTask(IMessagingContext messagingContext, CancellationToken cancellationToken = default) : base(messagingContext, cancellationToken)
        {
            tsc = new TaskCompletionSource<bool>();
        }

        public Task Completion => tsc.Task;

        public TaskAwaiter GetAwaiter() => Completion.GetAwaiter();

        protected void SetCompleted() => tsc.SetResult(true);

        protected void SetCanceled() => tsc.SetCanceled();

        protected void SetException(Exception exception) => tsc.SetException(exception);
    }
}
