using FakeItEasy;
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
        public void UseBuffers_Registers_Sending_And_Receiving_Buffer()
        {
            serviceProvider.RegisterService(typeof(MemoryPool<byte>), (p, s) => A.Fake<MemoryPool<byte>>());
            serviceProvider.UseBuffers(1024, 1024);

            var receivingBuffer = serviceProvider.GetService<IReceivingBuffer>();
            var sendingBuffer = serviceProvider.GetService<ISendingBuffer>();

            Assert.NotNull(receivingBuffer);
            Assert.NotNull(sendingBuffer);
        }

        [Fact]
        public void UseMessageBuffer_Register_MessageBuffer_And_MessageSender()
        {
            serviceProvider.RegisterService(typeof(ISendingBuffer), (p, s) => A.Fake<ISendingBuffer>());
            serviceProvider.RegisterService(typeof(ISerializer<string>), (p, s) => A.Fake<ISerializer<string>>());
            serviceProvider.UseMessageBuffer<string>();

            var messageSender = serviceProvider.GetService<IMessageSender<string>>();
            var messageBuffer = serviceProvider.GetService<IMessageBuffer<string>>();

            Assert.NotNull(messageSender);
            Assert.NotNull(messageBuffer);
        }
    }
}
