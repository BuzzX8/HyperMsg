using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageWaiter<T>
    {
        private readonly Queue<T> messageQueue = new Queue<T>();
        private readonly object sync = new object();

        private TaskCompletionSource<T> completionSource;

        public Task<T> WaitAsync(CancellationToken cancellationToken)
        {
            lock (sync)
            {
                if (messageQueue.Count > 0)
                {
                    return Task.FromResult(messageQueue.Dequeue());
                }

                cancellationToken.Register(CancelWaitTask);

                return (completionSource = new TaskCompletionSource<T>()).Task;
            }
        }

        private void CancelWaitTask()
        {
            lock (sync)
            {
                completionSource.SetCanceled();
                completionSource = null;
            }
        }

        public void SetMessage(T message)
        {
            lock (sync)
            {
                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                if (completionSource == null)
                {
                    messageQueue.Enqueue(message);
                    return;
                }

                completionSource.SetResult(message);
                completionSource = null;
            }
        }
    }
}
