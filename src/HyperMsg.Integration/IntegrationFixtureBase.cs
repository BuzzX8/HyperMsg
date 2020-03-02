namespace HyperMsg.Integration
{
    public abstract class IntegrationFixtureBase
    {
        private readonly ConfigurableServiceProvider serviceProvider;

        protected IntegrationFixtureBase(int receivingBufferSize, int transmittingBufferSize)
        {
            serviceProvider = new ConfigurableServiceProvider();
            serviceProvider.UseCoreServices(receivingBufferSize, transmittingBufferSize);
        }

        protected IConfigurable Configurable => serviceProvider;

        protected IMessageSender MessageSender => serviceProvider.GetService<IMessageSender>();

        protected IMessageHandlerRegistry HandlerRegistry => serviceProvider.GetService<IMessageHandlerRegistry>();

        protected TService GetService<TService>() => serviceProvider.GetService<TService>();
    }
}