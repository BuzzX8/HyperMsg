namespace HyperMsg.Integration
{
    public abstract class IntegrationFixtureBase
    {
        private readonly ServiceProvider serviceProvider;

        protected IntegrationFixtureBase(int receivingBufferSize, int transmittingBufferSize)
        {
            serviceProvider = new ServiceProvider();
            serviceProvider.AddCoreServices(receivingBufferSize, transmittingBufferSize);
        }

        protected IConfigurable Configurable => serviceProvider;

        protected IMessageSender MessageSender => serviceProvider.GetService<IMessageSender>();

        protected IMessageObservable HandlerRegistry => serviceProvider.GetService<IMessageObservable>();

        protected TService GetService<TService>() => serviceProvider.GetService<TService>();
    }
}