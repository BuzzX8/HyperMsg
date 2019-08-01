using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Integration
{
    public abstract class IntegrationFixtureBase<T>
    {
        private readonly ConfigurableServiceProvider serviceProvider;
        private bool configured;

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
        }

        protected IConfigurable Configurable => serviceProvider;

        protected IMessageSender<T> MessageSender
        {
            get
            {
                ConfigureServiceProvider();
                return messageSender.Value;
            }
        }

        protected IMessageBuffer<T> MessageBuffer
        {
            get
            {
                ConfigureServiceProvider();
                return messageBuffer.Value;
            }
        }

        protected IMessageHandlerRegistry<T> HandlerRegistry
        {
            get
            {
                ConfigureServiceProvider();
                return handlerRegistry.Value;
            }
        }

        protected ITransport Transport
        {
            get
            {
                ConfigureServiceProvider();
                return transport.Value;
            }
        }

        private void ConfigureServiceProvider()
        {
            if (configured)
            {
                return;
            }

            serviceProvider.UseCoreServices<T>(1024, 1024);
            ConfigureSerializer(serviceProvider);
            ConfigureTransport(serviceProvider);
            configured = true;
        }

        protected TService GetService<TService>() => serviceProvider.GetService<TService>();

        protected abstract void ConfigureSerializer(IConfigurable configurable);

        protected abstract void ConfigureTransport(IConfigurable configurable);

        protected virtual Task OpenTransportAsync(CancellationToken cancellationToken = default) => Transport.ProcessCommandAsync(TransportCommand.Open, cancellationToken);

        protected virtual Task CloseTransportAsync(CancellationToken cancellationToken = default) => Transport.ProcessCommandAsync(TransportCommand.Close, cancellationToken);
    }
}