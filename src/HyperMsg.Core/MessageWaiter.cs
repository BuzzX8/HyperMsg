using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageWaiter<T>
    {
        private readonly Queue<T> messageQueue = new Queue<T>();
        private TaskCompletionSource<T> completionSource;

        public Task<T> WaitAsync(CancellationToken cancellationToken)
        {
            if (messageQueue.Count > 0)
            {                
                return Task.FromResult(messageQueue.Dequeue());
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
                messageQueue.Enqueue(message);
                return;
            }

            completionSource.SetResult(message);
            completionSource = null;
        }
    }
}
