using System;
using System.Threading.Tasks;

namespace HyperMsg
{
    public abstract class MessagingTask<TResult> : MessagingObject, IMessagingTask<TResult>
    {
        private readonly TaskCompletionSource<TResult> completionSource;

        protected MessagingTask(IMessagingContext messagingContext) : base(messagingContext)
        {
            completionSource = new TaskCompletionSource<TResult>();            
        }

        public Task<TResult> Completion => completionSource.Task;

        protected void Start()
        {
            RegisterAutoDisposables();
            BeginAsync().ContinueWith(OnBeginAsyncCompleted);
        }

        protected virtual Task BeginAsync() => Task.CompletedTask;

        protected void SetResult(TResult result)
        {
            completionSource.SetResult(result);
            Dispose();
        }

        protected void SetException(Exception exception)
        {
            completionSource.SetException(exception);
            Dispose();
        }

        private void OnBeginAsyncCompleted(Task beginAsync)
        {
            if (beginAsync.Exception != null)
            {
                SetException(beginAsync.Exception);
            }
        }

        public override void Dispose()
        {
            completionSource.TrySetCanceled();
            base.Dispose();
        }
    }
}
