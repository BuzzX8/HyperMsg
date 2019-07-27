using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Integration
{
    public abstract class IntegrationFixtureBase<T>
    {
        private readonly ConfigurableServiceProvider serviceProvider;

        private readonly Lazy<IMessageSender<T>> messageSender;
        private readonly Lazy<IMessageBuffer<T>> messageBuffer;
        private readonly Lazy<IMessageHandlerRegistry<T>> handlerRegistry;
        private readonly Lazy<ITransport> transport;

        protected IntegrationFixtureBase()
        {
            serviceProvider = new ConfigurableServiceProvider();
            messageSender = new Lazy<IMessageSender<T>>(() => serviceProvider.GetService<IMessageSender<T>>());
            messageBuffer = new Lazy<IMessageBuffer<T>>(() => serviceProvider.GetService<IMessageBuffer<T>>());
            handlerRegistry = new Lazy<IMessageHandlerRegistry<T>>(() => serviceProvider.GetService<IMessageHandlerRegistry<T>>());
            transport = new Lazy<ITransport>(() => serviceProvider.GetService<ITransport>());

            serviceProvider.UseCoreServices<T>(1024, 1024);
            ConfigureSerializer(serviceProvider);
            ConfigureTransport(serviceProvider);
        }

        protected IMessageSender<T> MessageSender => messageSender.Value;

        protected IMessageBuffer<T> MessageBuffer => messageBuffer.Value;

        protected IMessageHandlerRegistry<T> HandlerRegistry => handlerRegistry.Value;

        protected ITransport Transport => transport.Value;

        protected abstract void ConfigureSerializer(IConfigurable configurable);

        protected abstract void ConfigureTransport(IConfigurable configurable);

        protected virtual Task OpenTransportAsync(CancellationToken cancellationToken = default) => Transport.ProcessCommandAsync(TransportCommand.Open, cancellationToken);

        protected virtual Task CloseTransportAsync(CancellationToken cancellationToken = default) => Transport.ProcessCommandAsync(TransportCommand.Close, cancellationToken);
    }
}