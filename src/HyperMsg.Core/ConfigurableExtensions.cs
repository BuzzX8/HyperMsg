using System;
using System.Buffers;

namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void UseCoreServices<T>(this IConfigurable configurable, int receivingBufferSize, int sendingBufferSize)
        {
            configurable.UseSharedMemoryPool();
            configurable.UseBuffers(receivingBufferSize, sendingBufferSize);
            
            configurable.UseMessageBuffer<T>();
        }

        public static void UseSharedMemoryPool(this IConfigurable configurable) => configurable.RegisterService(typeof(MemoryPool<byte>), (p, s) => MemoryPool<byte>.Shared);

        public static void UseBuffers(this IConfigurable configurable, int receivingBufferSize, int sendingBufferSize)
        {
            const string ReceivingBufferSetting = "ReceivingBufferSize";
            const string SendingBufferSetting = "SendingBufferSize";

            configurable.AddSetting(ReceivingBufferSetting, receivingBufferSize);
            configurable.AddSetting(SendingBufferSetting, sendingBufferSize);
            configurable.RegisterService(typeof(IReceivingBuffer), (p, s) =>
            {
                var bufferSize = (int)s[ReceivingBufferSetting];
                var memoryPool = (MemoryPool<byte>)p.GetService(typeof(MemoryPool<byte>));

                return new Buffer(memoryPool.Rent(bufferSize));
            });
            configurable.RegisterService(typeof(ISendingBuffer), (p, s) =>
            {
                var bufferSize = (int)s[SendingBufferSetting];
                var memoryPool = (MemoryPool<byte>)p.GetService(typeof(MemoryPool<byte>));

                return new Buffer(memoryPool.Rent(bufferSize));
            });
        }

        public static void UseMessageBuffer<T>(this IConfigurable configurable)
        {
            configurable.RegisterService(new[] { typeof(IMessageSender<T>), typeof(IMessageBuffer<T>) }, (p, s) =>
            {
                var sendingBuffer = (ISendingBuffer)p.GetService(typeof(ISendingBuffer));
                var serializer = (ISerializer<T>)p.GetService(typeof(ISerializer<T>));
                
                return new MessageBuffer<T>(sendingBuffer.Writer, serializer.Serialize, sendingBuffer.FlushAsync);
            });
        }

        public static void UseMessageHandlerAggregate<T>(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IMessageHandlerRegistry<T>), (p, s) =>
            {
                return new MessageHandlerRegistry<T>();
            });

            configurable.RegisterService(typeof(MessageHandler<T>), (p, s) =>
            {
                var aggregate = (MessageHandlerRegistry<T>)p.GetService(typeof(IMessageHandlerRegistry<T>));

                return (MessageHandler<T>)aggregate.HandleAsync;
            });
        }

        public static void UseBackgrounReceiver<T>(this IConfigurable configurable)
        {
            configurable.RegisterConfigurator((p, s) =>
            {
                var serializer = (ISerializer<T>)p.GetService(typeof(ISerializer<T>));
                var bufferReader = (IBufferReader<byte>)p.GetService(typeof(IBufferReader<byte>));
                var messageHandler = (MessageHandler<T>)p.GetService(typeof(MessageHandler<T>));
                var observer = new MessageBufferObserver<T>(serializer.Deserialize, bufferReader);
                observer.MessageDeserialized += new AsyncAction<T>(messageHandler);
                //var bgReceiver = new TransportWorker(observer.CheckBufferAsync);

                //var transport = (ITransport)p.GetService(typeof(ITransport));
                //transport.TransportEvent += bgReceiver.HandleTransportEventAsync;
            });
        }
    }
}
