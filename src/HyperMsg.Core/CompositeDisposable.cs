using System;

namespace HyperMsg
{
    public class CompositeDisposable : IDisposable
    {
        private readonly IDisposable[] disposables;

        public CompositeDisposable(params IDisposable[] disposables) => this.disposables = disposables;

        public void Dispose()
        {
            foreach(var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}
