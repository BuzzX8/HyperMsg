using FakeItEasy;
using System.Buffers;
using Xunit;

namespace HyperMsg
{
    public class ServiceRegistryExtensionsTests
    {
        private readonly ServiceContainer serviceProvider = new ServiceContainer();

        [Fact]
        public void UseSharedMemoryPool_Registers_Shared_MemoryPool()
        {
            serviceProvider.AddSharedMemoryPool();

            var memoryPool = serviceProvider.GetService<MemoryPool<byte>>();
            Assert.Same(MemoryPool<byte>.Shared, memoryPool);
        }

        [Fact]
        public void UseBufferContext_Registers_BufferContext()
        {
            serviceProvider.Add(typeof(MemoryPool<byte>), (p) => A.Fake<MemoryPool<byte>>());
            serviceProvider.AddBufferContext(100, 100);

            var context = serviceProvider.GetService<IBufferContext>();

            Assert.NotNull(context);
        }

        [Fact]
        public void UseMessageBroker_Registers_MessagingContext_MessageSender_And_MessageObservable()
        {
            serviceProvider.AddMessageBroker();

            var messageSender = serviceProvider.GetService<IMessageSender>();
            var observable = serviceProvider.GetService<IMessageObservable>();
            var context = serviceProvider.GetService<IMessagingContext>();

            Assert.NotNull(messageSender);
            Assert.NotNull(observable);
            Assert.NotNull(context);
        }
    }
}
