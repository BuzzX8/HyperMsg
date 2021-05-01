﻿using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
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
            var observable = provider.GetService<IMessageHandlersRegistry>();
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
            services.AddBufferContext();
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
        public void AddMessageHandler_Registers_Message_Handler()
        {
            var message = Guid.NewGuid();
            var handler = A.Fake<Action<Guid>>();
            var host = ServiceHost.CreateDefault(services => services.AddMessageHandler(handler));
            host.StartAsync().Wait();

            var sender = host.GetRequiredService<IMessageSender>();
            sender.Send(message);

            A.CallTo(() => handler.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public void AddMessageHandler_Disposes_Message_Handler_Registration()
        {
            var message = Guid.NewGuid();
            var handler = A.Fake<Action<Guid>>();
            var host = ServiceHost.CreateDefault(services => services.AddMessageHandler(handler));
            host.StartAsync().Wait();

            var sender = host.GetRequiredService<IMessageSender>();
            host.StopAsync().Wait();
            host.Dispose();
            sender.Send(message);

            A.CallTo(() => handler.Invoke(message)).MustNotHaveHappened();
        }
    }
}