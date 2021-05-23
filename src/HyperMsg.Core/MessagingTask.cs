using System;
using System.Threading.Tasks;

namespace HyperMsg
{
    public abstract class MessagingTaskBase : MessagingObject
    {
        protected MessagingTaskBase(IMessagingContext messagingContext) : base(messagingContext)
        {
        }

        protected void Start()
        {
            RegisterAutoDisposables();
            BeginAsync().ContinueWith(OnBeginAsyncCompleted);
        }

        protected virtual Task BeginAsync() => Task.CompletedTask;

        protected abstract void SetException(Exception exception);

        private void OnBeginAsyncCompleted(Task beginAsync)
        {
            if (beginAsync.Exception != null)
            {
                SetException(beginAsync.Exception);
            }
        }
    }

    public abstract class MessagingTask : MessagingTaskBase
    {
        private readonly TaskCompletionSource<bool> completionSource;

        protected MessagingTask(IMessagingContext messagingContext) : base(messagingContext)
        {
        }

        public Task Completion => completionSource.Task;

        protected void SetCompleted()
        {
            completionSource.SetResult(true);
            Dispose();
        }

        protected override void SetException(Exception exception)
        {
            completionSource.SetException(exception);
            Dispose();
        }
    }

    public abstract class MessagingTask<TResult> : MessagingTaskBase, IMessagingTask<TResult>
    {
        private readonly TaskCompletionSource<TResult> completionSource;

        protected MessagingTask(IMessagingContext messagingContext) : base(messagingContext)
        {
            completionSource = new TaskCompletionSource<TResult>();            
        }

        public Task<TResult> Completion => completionSource.Task;

        protected void SetResult(TResult result)
        {
            completionSource.SetResult(result);
            Dispose();
        }

        protected override void SetException(Exception exception)
        {
            completionSource.SetException(exception);
            Dispose();
        }

        public override void Dispose()
        {
            completionSource.TrySetCanceled();
            base.Dispose();
        }
    }
}
