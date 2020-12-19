using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using System.Threading;
using Xunit;

namespace HyperMsg.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        private readonly ServiceCollection services = new ServiceCollection();
        private readonly static TimeSpan waitTimeout = TimeSpan.FromSeconds(5);

        [Fact]
        public void AddMessageBroker_Adds_Sender_Observable_And_Context()
        {
            services.AddMessageBroker();
            var provider = services.BuildServiceProvider();

            var sender = provider.GetService<IMessageSender>();
            var observable = provider.GetService<IMessageObservable>();
            var context = provider.GetService<IMessagingContext>();

            Assert.NotNull(sender);
            Assert.NotNull(observable);
            Assert.NotNull(context);
        }

        [Fact]
        public void AddSharedMemoryPool_Adds_Memory_Pool()
        {
            services.AddSharedMemoryPool();
            var provider = services.BuildServiceProvider();

            var pool = provider.GetService<MemoryPool<byte>>();

            Assert.NotNull(pool);
        }

        [Fact]
        public void AddBufferContext_Adds_BufferContext()
        {
            services.AddSharedMemoryPool();
            services.AddBufferContext(100, 100);
            var provider = services.BuildServiceProvider();

            var context = provider.GetService<IBufferContext>();

            Assert.NotNull(context);
        }

        [Fact]
        public void AddBufferFactory_Adds_BufferFactory()
        {
            services.AddSharedMemoryPool();
            services.AddBufferFactory();
            var provider = services.BuildServiceProvider();

            var factory = provider.GetService<IBufferFactory>();

            Assert.NotNull(factory);
        }

        [Fact]
        public void AddObservers_Applies_Observer_Configurator()
        {
            var wasInvoked = false;
            var waitEvent = new ManualResetEventSlim();
            var builder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddCoreServices(100, 100);
                    services.AddObservers((provider, observable) =>
                    {
                        wasInvoked = true;
                        waitEvent.Set();
                    });
                });

            var runTask = builder.Build().RunAsync();
            waitEvent.Wait(waitTimeout);

            Assert.True(wasInvoked);
        }

        [Fact]
        public void AddObservers_Applies_ComponentObserver_Configurator()
        {
            var wasInvoked = false;
            var waitEvent = new ManualResetEventSlim();
            var builder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddCoreServices(100, 100);
                    services.AddObservers<MessageBroker>((component, observable) =>
                    {
                        wasInvoked = true;
                        waitEvent.Set();
                    });
                });

            var runTask = builder.Build().RunAsync();
            waitEvent.Wait(waitTimeout);

            Assert.True(wasInvoked);
        }
    }
}