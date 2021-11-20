using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    internal class ResultRepository : IResultRepository
    {
        private readonly IDictionary<object, object> pendingPromises = new Dictionary<object, object>();
        
        public Task<T> GetResultPromise<T>(object key, CancellationToken cancellationToken = default)
        {
            if (!pendingPromises.ContainsKey(key))
            {
                var tsc = new TaskCompletionSource<T>();
                pendingPromises.Add(key, tsc);
            }

            return (pendingPromises[key] as TaskCompletionSource<T>).Task;
        }

        public void SetResult<T>(object key, T result)
        {
            if (!pendingPromises.ContainsKey(key))
            {
                var tsc1 = new TaskCompletionSource<T>();
                pendingPromises.Add(key, tsc1);
                tsc1.SetResult(result);
                return;
            }

            if (pendingPromises.ContainsKey(key) && pendingPromises[key] is TaskCompletionSource<T> tsc)
            {
                tsc.SetResult(result);
            }
        }

        public void SetError(object key, Exception exception)
        {
            throw new NotImplementedException();
        }
    }

    public interface IResultRepository
    {
        Task<T> GetResultPromise<T>(object key, CancellationToken cancellationToken = default);

        void SetResult<T>(object key, T result);

        void SetError(object key, Exception exception);
    }
}