using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageWaiter<T>
    {        
        private TaskCompletionSource<T> completionSource;
        private object message;

        public Task<T> WaitAsync(CancellationToken cancellationToken)
        {
            if (message != null)
            {
                return Task.FromResult((T)message);
            }

            return (completionSource = new TaskCompletionSource<T>()).Task;            
        }

        public void SetMessage(T message)
        {
            if(completionSource == null)
            {
                this.message = message;
                return;
            }

            completionSource.SetResult(message);
            completionSource = null;
        }
    }
}
