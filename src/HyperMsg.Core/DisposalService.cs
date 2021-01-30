using System;
using System.Collections.Generic;

namespace HyperMsg
{
    internal class DisposalService : IDisposable
    {
        private readonly List<IDisposable> disposables = new();

        internal void AddDisposable(IDisposable disposable) => disposables.Add(disposable);

        public void Dispose() => disposables.ForEach(d => d.Dispose());
    }
}
