namespace HyperMsg
{
    public class Runtime : IRuntime
    {
        public Runtime(IServiceProvider serviceProvider)
        {
            SendingBroker = new MessageBroker();
            ReceivingBroker = new MessageBroker();
            ServiceProvider = serviceProvider;            
        }

        public IBroker SendingBroker { get; }

        public IBroker ReceivingBroker { get; }

        public IServiceProvider ServiceProvider { get; }
    }
}
