namespace HyperMsg
{
    public class MessagingObjectTests
    {
        private readonly MessageBroker messageBroker;
        private readonly MessagingObjectImpl messagingObject;

        public MessagingObjectTests()
        {
            messageBroker = new();
            messagingObject = new(messageBroker);
        }
    }

    internal class MessagingObjectImpl : MessagingObject
    {
        public MessagingObjectImpl(IMessagingContext messagingContext) : base(messagingContext)
        {
        }
    }
}
