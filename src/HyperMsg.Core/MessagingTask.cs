using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public abstract class MessagingTask<TResult> : MessagingObject
    {
        private readonly TaskCompletionSource<TResult> completionSource;

        protected MessagingTask(IMessagingContext messagingContext, CancellationToken cancellationToken = default) : base(messagingContext)
        {
            CancellationToken = cancellationToken;            
            completionSource = new TaskCompletionSource<TResult>();            
        }

        public bool IsCompleted => completionSource.Task.IsCompleted;

        public TaskAwaiter<TResult> GetAwaiter() => completionSource.Task.GetAwaiter();

        public Task<TResult> AsTask() => completionSource.Task;

        protected CancellationToken CancellationToken { get; }

        public TResult Result => completionSource.Task.Result;
        
        protected void Complete(TResult result)
        {
            completionSource.SetResult(result);
            Dispose();
        }
    }
}
