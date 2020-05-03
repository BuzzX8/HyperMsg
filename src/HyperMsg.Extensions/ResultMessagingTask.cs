using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessagingTask<T> : MessagingTaskBase
    {
        private readonly TaskCompletionSource<T> tsc;

        public MessagingTask(IMessageObservable messageObservable) : base(messageObservable)
        {
            tsc = new TaskCompletionSource<T>();
        }

        public Task<T> Completion => tsc.Task;

        public TaskAwaiter<T> GetAwaiter() => Completion.GetAwaiter();

        protected void SetResult(T result)
        { }

        protected void SetCanceled()
        { }

        protected void SetException(Exception exception)
        { }
    }
}
