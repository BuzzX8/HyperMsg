namespace HyperMsg
{
    public class MessagingTaskTests
    {
    }

    public class MessagingTaskMock : MessagingTask
    {
        public MessagingTaskMock(IMessagingContext messagingContext) : base(messagingContext)
        {
        }
    }
}
