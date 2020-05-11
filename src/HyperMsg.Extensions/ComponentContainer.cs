using System;
using System.Collections.Generic;

namespace HyperMsg
{
    internal class ComponentContainer : IDisposable
    {
        private readonly IMessagingContext messagingContext;
        private readonly List<IDisposable> subscriptions;

        internal ComponentContainer(IMessagingContext messagingContext)
        {
            this.messagingContext = messagingContext ?? throw new ArgumentNullException(nameof(messagingContext));
            subscriptions = new List<IDisposable>();
        }

        internal void Add(IMessagingComponent messagingComponent)
        {
            subscriptions.AddRange(messagingComponent.Attach(messagingContext));
        }

        public void Dispose()
        {
            subscriptions.ForEach(s => s.Dispose());
        }
    }
}
