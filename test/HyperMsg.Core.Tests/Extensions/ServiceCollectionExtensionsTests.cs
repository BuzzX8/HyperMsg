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

        [Fact]
        public void AddSerializationComponent_Invokes_BufferObserver_And_BufferTransmitter()
        {
            var serializer = A.Fake<Action<IBufferWriter<byte>, Guid>>();
            var bufferTransmitter = A.Fake<Action<IBuffer>>();

            services.AddMessagingServices();
            services.AddBufferDataTransmitObserver(bufferTransmitter);
            services.AddSerializationComponent(serializer);
            var host = new Host(services);
            var data = Guid.NewGuid();            
            host.Start();

            var sender = host.Services.GetRequiredService<IMessageSender>();
            sender.Transmit(data);

            A.CallTo(() => serializer.Invoke(A<IBufferWriter<byte>>._, data)).MustHaveHappened();
            A.CallTo(() => bufferTransmitter.Invoke(A<IBuffer>._)).MustHaveHappened();
        }

        [Fact]
        public void AddDeserializationComponent_()
        {
            var actualData = Guid.Empty;
            services.AddMessagingServices();
            services.AddDeserializationComponent(buffer =>
            {
                var bytes = buffer.ToArray();
                return (bytes.Length, new Guid(bytes));
            });
            services.AddReceiveObserver<Guid>(data => actualData = data);
            var host = new Host(services);
            var data = Guid.NewGuid();
            host.Start();

            var sender = host.Services.GetRequiredService<IMessageSender>();
            var bufferContext = host.Services.GetRequiredService<IBufferContext>();
            var buffer = bufferContext.ReceivingBuffer;
            buffer.Writer.Write(data.ToByteArray());

            sender.BufferReceivedData(buffer);

            Assert.Equal(data, actualData);
        }
    }
}