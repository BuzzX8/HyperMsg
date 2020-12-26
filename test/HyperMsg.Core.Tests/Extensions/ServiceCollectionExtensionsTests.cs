using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Buffers;
using Xunit;

namespace HyperMsg.Extensions
{
    public class ServiceCollectionExtensionsTests
    {
        private readonly ServiceCollection services = new();

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
        public void AddObservers_Invokes_Configuration_Delegate()
        {
            var configurationDelegate = A.Fake<Action<IServiceProvider, IMessageObservable>>();
            services.AddMessageBroker();            
            services.AddObservers(configurationDelegate);
            var host = new Host(services);
            host.Start();

            A.CallTo(() => configurationDelegate.Invoke(A<IServiceProvider>._, A<IMessageObservable>._)).MustHaveHappened();
        }

        [Fact]
        public void AddObservers_Invokes_Configuration_Delegate2()
        {
            var configurationDelegate = A.Fake<Action<IMessageObservable>>();
            services.AddMessageBroker();
            services.AddObservers(configurationDelegate);
            var host = new Host(services);
            host.Start();

            A.CallTo(() => configurationDelegate.Invoke(A<IMessageObservable>._)).MustHaveHappened();
        }

        [Fact]
        public void AddObservers_Invokes_Configuration_Delegate3()
        {
            var configurationDelegate = A.Fake<Action<MessageBroker, IMessageObservable>>();
            services.AddMessageBroker();
            services.AddObservers(configurationDelegate);
            var host = new Host(services);
            host.Start();

            A.CallTo(() => configurationDelegate.Invoke(A<MessageBroker>._, A<IMessageObservable>._)).MustHaveHappened();
        }
    }
}