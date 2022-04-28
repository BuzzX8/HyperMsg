namespace HyperMsg
{
    public class Runtime : IRuntime
    {
        public Runtime(IServiceProvider serviceProvider)
        {
            Sender = new MessageBroker();
            Receiver = new MessageBroker();
            ServiceProvider = serviceProvider;            
        }

        public IBroker Sender { get; }

        public IBroker Receiver { get; }

        public IServiceProvider ServiceProvider { get; }
    }
}
