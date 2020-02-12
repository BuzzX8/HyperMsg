﻿using FakeItEasy;
using System.Buffers;
using Xunit;

namespace HyperMsg
{
    public class ConfigurableExtensionsTests
    {
        private readonly ConfigurableServiceProvider serviceProvider = new ConfigurableServiceProvider();

        [Fact]
        public void UseSharedMemoryPool_Registers_Shared_MemoryPool()
        {
            serviceProvider.UseSharedMemoryPool();

            var memoryPool = serviceProvider.GetService<MemoryPool<byte>>();
            Assert.Same(MemoryPool<byte>.Shared, memoryPool);
        }

        [Fact]
        public void UseBufferContext_Registers_BufferContext()
        {
            serviceProvider.RegisterService(typeof(MemoryPool<byte>), (p, s) => A.Fake<MemoryPool<byte>>());
            serviceProvider.UseBufferContext(1024, 1024);

            var context = serviceProvider.GetService<IBufferContext>();

            Assert.NotNull(context);
        }

        [Fact]
        public void UseMessageBroker_Registers_MessageSender_And_MessageHandlerRegistry()
        {
            serviceProvider.UseMessageBroker();

            var messageSender = serviceProvider.GetService<IMessageSender>();
            var handlerRegistry = serviceProvider.GetService<IMessageHandlerRegistry>();

            Assert.NotNull(messageSender);
            Assert.NotNull(handlerRegistry);
        }
    }
}
