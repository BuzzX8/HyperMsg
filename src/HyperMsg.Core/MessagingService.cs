using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessagingService : MessagingContextProxy, IHostedService, IDisposable
    {
        private readonly List<IDisposable> subscriptions;

        public MessagingService(IMessagingContext messagingContext) : base(messagingContext)
        {
            subscriptions = new();
        }

        private void RegisterDisposable(IDisposable disposable) => subscriptions.Add(disposable);

        protected void RegisterAutoDisposables()
        {
            foreach (var handle in GetAutoDisposables())
            {
                RegisterDisposable(handle);
            }
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            RegisterAutoDisposables();
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public virtual void Dispose() => subscriptions.ForEach(s => s.Dispose());
    }
}
