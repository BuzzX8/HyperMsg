using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageWaiter<T>
    {        
        private TaskCompletionSource<T> completionSource;
        private bool isMessageSet = false;
        private T message;

        public Task<T> WaitAsync(CancellationToken cancellationToken)
        {
            if (isMessageSet)
            {
                isMessageSet = false;
                return Task.FromResult(message);
            }

            return (completionSource = new TaskCompletionSource<T>()).Task;            
        }

        public void SetMessage(T message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if(completionSource == null)
            {
                this.message = message;
                isMessageSet = true;
                return;
            }

            completionSource.SetResult(message);
            completionSource = null;
        }
    }
}
