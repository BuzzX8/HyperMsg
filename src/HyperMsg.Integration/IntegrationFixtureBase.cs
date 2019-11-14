using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Integration
{
    public abstract class IntegrationFixtureBase
    {
        private readonly ConfigurableServiceProvider serviceProvider = new ConfigurableServiceProvider();

        protected IConfigurable Configurable => serviceProvider;

        protected IMessageSender MessageSender => serviceProvider.GetService<IMessageSender>();

        protected IMessageHandlerRegistry HandlerRegistry => serviceProvider.GetService<IMessageHandlerRegistry>();

        protected TService GetService<TService>() => serviceProvider.GetService<TService>();

        protected abstract void ConfigureSerializer(IConfigurable configurable);

        protected abstract void ConfigureTransport(IConfigurable configurable);

        protected virtual Task OpenTransportAsync(CancellationToken cancellationToken = default) => MessageSender.SendAsync(TransportCommand.Open, cancellationToken);

        protected virtual Task CloseTransportAsync(CancellationToken cancellationToken = default) => MessageSender.SendAsync(TransportCommand.Close, cancellationToken);
    }
}