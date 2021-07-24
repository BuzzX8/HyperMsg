using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    /// <summary>
    /// Implementation of hosted service which exists in scope of MessagingContext
    /// </summary>
    public class MessagingService : MessagingContextProxy, IHostedService, IDisposable
    {
        private readonly List<IDisposable> subscriptionHandles;

        public MessagingService(IMessagingContext messagingContext) : base(messagingContext) => subscriptionHandles = new();

        /// <summary>
        /// Should return subscription handles for message handlers which must exist during service lifetime.
        /// </summary>
        /// <returns>List of subscription handles.</returns>
        protected virtual IEnumerable<IDisposable> GetSubscriptionHandles() => Enumerable.Empty<IDisposable>();

        private void RegisterSubscriptionHandles()
        {
            if (subscriptionHandles.Count > 0)
            {
                return;
            }

            subscriptionHandles.AddRange(GetSubscriptionHandles());
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            RegisterSubscriptionHandles();
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public virtual void Dispose() => subscriptionHandles.ForEach(s => s.Dispose());
    }
}
