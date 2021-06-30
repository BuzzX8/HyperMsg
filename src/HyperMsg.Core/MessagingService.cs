using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessagingService : MessagingContextProxy, IHostedService, IDisposable
    {
        private readonly List<IDisposable> childDisposables;

        public MessagingService(IMessagingContext messagingContext) : base(messagingContext) => childDisposables = new();

        protected virtual IEnumerable<IDisposable> GetChildDisposables() => Enumerable.Empty<IDisposable>();

        protected void RegisterChildDisposables()
        {
            foreach (var disposable in GetChildDisposables())
            {
                childDisposables.Add(disposable);
            }
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            RegisterChildDisposables();
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public virtual void Dispose() => childDisposables.ForEach(s => s.Dispose());
    }
}
