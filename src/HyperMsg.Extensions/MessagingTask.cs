using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessagingTask : MessagingTaskBase
    {
        private readonly TaskCompletionSource<bool> tsc;

        public MessagingTask(IMessageObservable messageObservable) : base(messageObservable)
        {
            tsc = new TaskCompletionSource<bool>();
        }

        public Task Completion => tsc.Task;

        public TaskAwaiter GetAwaiter() => Completion.GetAwaiter();

        protected void SetCompleted()
        { }

        protected void SetCanceled()
        { }

        protected void SetException(Exception exception)
        { }
    }
}
