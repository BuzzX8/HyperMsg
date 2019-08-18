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
            configurable.UseMessageHandlerRegistry<T>();
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

        public static void UseMessageHandlerRegistry<T>(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IMessageHandlerRegistry<T>), (p, s) =>
            {
                var receivingBuffer = (IReceivingBuffer)p.GetService(typeof(IReceivingBuffer));
                var serializer = (ISerializer<T>)p.GetService(typeof(ISerializer<T>));
                var bufferObserver = new MessageBufferObserver<T>(serializer.Deserialize);
                var registry = new MessageHandlerRegistry<T>();

                receivingBuffer.FlushRequested += bufferObserver.CheckBufferAsync;
                bufferObserver.MessageDeserialized += registry.HandleAsync;
                return registry;
            });
        }
    }
}
