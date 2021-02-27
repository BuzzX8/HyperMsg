using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HyperMsg.Extensions
{
    public static class TaskExtensions
    {
        public static Task OnSuccessfullyComplete(this Task task, Action completeHandler)
        {
            return task.ContinueWith(t => completeHandler.Invoke(), TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
        }

        public static Task OnSuccessfullyComplete(this Task task, Func<Task> completeHandler)
        {
            return task.ContinueWith(async _ => await completeHandler.Invoke(), TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
        }

        public static Task OnSuccessfullyComplete<T>(this Task<T> task, Action<T> completeHandler)
        {
            return task.ContinueWith(t => completeHandler.Invoke(t.Result), TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
        }

        public static Task OnFault(this Task task, Action<IReadOnlyList<Exception>> faultHandler)
        {
            return task.ContinueWith(t =>
            {
                t.Exception.Flatten();
                faultHandler.Invoke(t.Exception.InnerExceptions);
            }, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}
