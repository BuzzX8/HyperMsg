using System;

namespace HyperMsg
{
    public abstract class MessagingContextProxy
    {
        protected MessagingContextProxy(IMessagingContext messagingContext) => (MessagingContext) = (messagingContext ?? throw new ArgumentNullException(nameof(messagingContext)));

        protected IMessagingContext MessagingContext { get; }

        protected IMessageHandlersRegistry HandlersRegistry => MessagingContext.HandlersRegistry;

        protected IMessageSender Sender => MessagingContext.Sender;
    }
}
