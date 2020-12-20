using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds core services required for messaging and buffering infrastructure (MessageSender, MessageObservable,
        /// receiving and transmitting buffer).
        /// </summary>
        /// <param name="services"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        public static IServiceCollection AddMessagingServices(this IServiceCollection services, int receivingBufferSize, int transmittingBufferSize)
        {
            return services.AddBufferContext(receivingBufferSize, transmittingBufferSize)
                .AddSharedMemoryPool()                
                .AddMessageBroker();            
        }

        /// <summary>
        /// Adds shared MemoryPool<byte> as service.
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddSharedMemoryPool(this IServiceCollection services) => services.AddSingleton(MemoryPool<byte>.Shared);

        /// <summary>
        /// Adds implementations for IBufferContext. Depends on MemoryPool<byte>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="receivingBufferSize">Size of receiving buffer.</param>
        /// <param name="transmittingBufferSize">Size of transmitting buffer.</param>
        public static IServiceCollection AddBufferContext(this IServiceCollection services, int receivingBufferSize, int transmittingBufferSize)
        {
            return services.AddSingleton(provider =>
            {
                var memoryPool = provider.GetRequiredService<MemoryPool<byte>>();

                var receivingBuffer = new Buffer(memoryPool.Rent(receivingBufferSize));
                var transmittingBuffer = new Buffer(memoryPool.Rent(transmittingBufferSize));

                return new BufferContext(receivingBuffer, transmittingBuffer) as IBufferContext;
            });
        }

        /// <summary>
        /// Adds implementation for IBufferFactory. Depends on MemoryPool<byte>
        /// </summary>
        /// <param name="services"></param>
        public static IServiceCollection AddBufferFactory(this IServiceCollection services) => services.AddSingleton(provider =>
        {
            var memoryPool = provider.GetRequiredService<MemoryPool<byte>>();
            return new BufferFactory(memoryPool) as IBufferFactory;
        });

        /// <summary>
        /// Adds implementation for services IMessagingContext, IMessageSender and IMessageObservable.
        /// </summary>
        /// <param name="serviceRegistry"></param>
        public static IServiceCollection AddMessageBroker(this IServiceCollection services)
        {
            var broker = new MessageBroker();
            return services.AddSingleton<IMessageSender>(broker)
                .AddSingleton<IMessageObservable>(broker)
                .AddSingleton<IMessagingContext>(broker);
        }

        public static IServiceCollection AddObservers(this IServiceCollection services, Action<IServiceProvider, IMessageObservable> configurationDelegate)
        {
            if (!services.Any(s => s.ServiceType == typeof(ConfiguratorCollection)))
            {
                services.AddSingleton(new ConfiguratorCollection());
            }

            var configurators = services.Single(s => s.ServiceType == typeof(ConfiguratorCollection)).ImplementationInstance as ConfiguratorCollection;
            configurators.Add(configurationDelegate);
            return services.AddHostedService<HyperMsgBootstrapper>();
        }

        public static IServiceCollection AddObservers(this IServiceCollection services, Action<IMessageObservable> configurationDelegate)
        {
            return services.AddObservers((provider, observable) => configurationDelegate.Invoke(observable));
        }

        public static IServiceCollection AddObservers<TComponent>(this IServiceCollection services, Action<TComponent, IMessageObservable> configurationDelegate, bool addComponent = true) where TComponent : class
        {
            if (addComponent)
            {
                services.AddSingleton<TComponent>();
            }

            return services.AddObservers((provider, observable) =>
            {
                var component = provider.GetRequiredService<TComponent>();
                configurationDelegate.Invoke(component, observable);
            });
        }

        public static IServiceCollection AddObserver<TMessage>(this IServiceCollection services, Func<Action<TMessage>> configurationFunc)
        {
            return services.AddObservers(observable => observable.Subscribe(configurationFunc.Invoke()));
        }

        public static IServiceCollection AddObserver<TComponent, TMessage>(this IServiceCollection services, Func<TComponent, Action<TMessage>> configurationDelegate) where TComponent : class
        {
            return services.AddObservers<TComponent>((component, observable) => observable.Subscribe(configurationDelegate.Invoke(component)));
        }

        public static IServiceCollection AddObserver<TMessage>(this IServiceCollection services, Func<AsyncAction<TMessage>> configurationFunc)
        {
            return services.AddObservers(observable => observable.Subscribe(configurationFunc.Invoke()));
        }

        public static IServiceCollection AddObserver<TComponent, TMessage>(this IServiceCollection services, Func<TComponent, AsyncAction<TMessage>> configurationDelegate) where TComponent : class
        {
            return services.AddObservers<TComponent>((component, observable) => observable.Subscribe(configurationDelegate.Invoke(component)));
        }

        public static IServiceCollection AddTransmitObserver<TMessage>(this IServiceCollection services, Func<Action<TMessage>> configurationDelegate)
        {
            return services.AddObservers(observable => observable.OnTransmit(configurationDelegate.Invoke()));
        }

        public static IServiceCollection AddTransmitObserver<TComponent, TMessage>(this IServiceCollection services, Func<TComponent, Action<TMessage>> configurationDelegate) where TComponent : class
        {
            return services.AddObservers<TComponent>((component, observable) => observable.OnTransmit(configurationDelegate.Invoke(component)));
        }

        public static IServiceCollection AddTransmitObserver<TMessage>(this IServiceCollection services, Func<AsyncAction<TMessage>> configurationDelegate)
        {
            return services.AddObservers(observable => observable.OnTransmit(configurationDelegate.Invoke()));
        }

        public static IServiceCollection AddTransmitObserver<TComponent, TMessage>(this IServiceCollection services, Func<TComponent, AsyncAction<TMessage>> configurationDelegate) where TComponent : class
        {
            return services.AddObservers<TComponent>((component, observable) => observable.OnTransmit(configurationDelegate.Invoke(component)));
        }

        public static IServiceCollection AddReceiveObserver<TMessage>(this IServiceCollection services, Func<Action<TMessage>> configurationFunc)
        {
            return services.AddObservers(observable => observable.OnReceived(configurationFunc.Invoke()));
        }

        public static IServiceCollection AddReceiveObserver<TMessage>(this IServiceCollection services, Func<AsyncAction<TMessage>> configurationFunc)
        {
            return services.AddObservers(observable => observable.OnReceived(configurationFunc.Invoke()));
        }

        public static IServiceCollection AddReceiveObserver<TComponent, TMessage>(this IServiceCollection services, Func<TComponent, Action<TMessage>> configurationFunc) where TComponent : class
        {
            return services.AddObservers<TComponent>((component, observable) => observable.OnReceived(configurationFunc.Invoke(component)));
        }

        public static IServiceCollection AddReceiveObserver<TComponent, TMessage>(this IServiceCollection services, Func<TComponent, AsyncAction<TMessage>> configurationFunc) where TComponent : class
        {
            return services.AddObservers<TComponent>((component, observable) => observable.OnReceived(configurationFunc.Invoke(component)));
        }

        public static IServiceCollection AddBufferDataTransmitObserver(this IServiceCollection services, Func<Action<IBuffer>> configurationDelegate)
        {
            return services.AddObservers(observable => observable.OnBufferDataTransmit(configurationDelegate.Invoke()));
        }

        public static IServiceCollection AddBufferDataTransmitObserver<TComponent>(this IServiceCollection services, Func<TComponent, Action<IBuffer>> configurationDelegate) where TComponent : class
        {
            return services.AddObservers<TComponent>((component, observable) => observable.OnBufferDataTransmit(configurationDelegate.Invoke(component)));
        }

        public static IServiceCollection AddBufferDataTransmitObserver(this IServiceCollection services, Func<AsyncAction<IBuffer>> configurationDelegate)
        {
            return services.AddObservers(observable => observable.OnBufferDataTransmit(configurationDelegate.Invoke()));
        }

        public static IServiceCollection AddBufferDataTransmitObserver<TComponent>(this IServiceCollection services, Func<TComponent, AsyncAction<IBuffer>> configurationDelegate) where TComponent : class
        {
            return services.AddObservers<TComponent>((component, observable) => observable.OnBufferDataTransmit(configurationDelegate.Invoke(component)));
        }
    }

    internal class HyperMsgBootstrapper : IHostedService
    {
        private readonly IServiceProvider provider;
        private readonly ConfiguratorCollection configurators;

        public HyperMsgBootstrapper(IServiceProvider provider, ConfiguratorCollection configurators)
        {
            this.provider = provider;
            this.configurators = configurators;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var observable = provider.GetRequiredService<IMessageObservable>();
            foreach (var configurator in configurators)
            {
                configurator.Invoke(provider, observable);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }

    internal class ConfiguratorCollection : List<Action<IServiceProvider, IMessageObservable>> { }
}
