using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    internal class ResultRepository
    {
        public Task<T> GetResultPromise<T>(object key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<T>(default);
        }

        public void SetResult<T>(object key, T result)
        {

        }
    }
}
