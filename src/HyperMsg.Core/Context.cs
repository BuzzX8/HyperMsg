namespace HyperMsg
{
    internal class Context : IContext
    {
        public Context()
        {
            Sender = new MessageBroker();
            Receiver = new MessageBroker();
        }

        public IBroker Sender { get; }

        public IBroker Receiver { get; }
    }
}
