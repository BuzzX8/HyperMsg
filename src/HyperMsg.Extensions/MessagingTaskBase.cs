using System;
using System.Threading;

namespace HyperMsg
{
    public abstract class MessagingTaskBase : MessagingObject
    {
        private readonly IDisposable cancelSubscription;

        protected MessagingTaskBase(IMessagingContext messagingContext, CancellationToken cancellationToken) : base(messagingContext)
        {            
            CancellationToken = cancellationToken;
            cancelSubscription = cancellationToken.Register(Dispose);
        }

        protected CancellationToken CancellationToken { get; }

        public override void Dispose()
        {
            base.Dispose();
            cancelSubscription?.Dispose();            
        }
    }
}
