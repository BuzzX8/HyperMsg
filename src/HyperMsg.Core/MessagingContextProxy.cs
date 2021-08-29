using System;

namespace HyperMsg
{
    public abstract class MessagingContextProxy
    {
        protected MessagingContextProxy(IMessagingContext messagingContext) => (MessagingContext) = (messagingContext ?? throw new ArgumentNullException(nameof(messagingContext)));

        protected IMessagingContext MessagingContext { get; }

        protected IHandlersRegistry HandlersRegistry => MessagingContext.HandlersRegistry;

        protected ISender Sender => MessagingContext.Sender;
    }
}
