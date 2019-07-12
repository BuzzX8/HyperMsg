using System;
using System.Buffers;

namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void UseCoreServices<T>(this IConfigurable configurable, int inputBufferSize, int outputBufferSize)
        {
            configurable.UseMessageHandlerAggregate<T>();
            configurable.UseBackgrounReceiver<T>();
            configurable.UseSharedMemoryPool();

            configurable.UseBufferReader(inputBufferSize);
            configurable.UseBufferWriter(outputBufferSize);
            configurable.UseMessageBuffer<T>();
        }

        public static void UseMessageBuffer<T>(this IConfigurable configurable)
        {
            configurable.RegisterService(new[] { typeof(IMessageSender<T>), typeof(IMessageBuffer<T>) }, (p, s) =>
            {
                var serializer = (ISerializer<T>)p.GetService(typeof(ISerializer<T>));
                var bufferWriter = (IBufferWriter<byte>)p.GetService(typeof(IBufferWriter<byte>));
                var flushHandler = (FlushHandler)p.GetService(typeof(FlushHandler));

                return new MessageBuffer<T>(bufferWriter, serializer.Serialize, new AsyncAction(flushHandler));

            });
        }

        public static void UseSharedMemoryPool(this IConfigurable configurable) => configurable.RegisterService(typeof(MemoryPool<byte>), (p, s) => MemoryPool<byte>.Shared);

        public static void UseBufferReader(this IConfigurable configurable, int bufferSize)
        {
            const string SettingName = "BufferReader.BufferSize";

            configurable.AddSetting(SettingName, bufferSize);
            configurable.RegisterService(typeof(IBufferReader), (p, s) =>
            {
                var buffSize = (int)s[SettingName];
                var memoryPool = (MemoryPool<byte>)p.GetService(typeof(MemoryPool<byte>));
                var transport = (ITransport)p.GetService(typeof(ITransport));
                var stream = transport.GetStream();
                
                return new BufferReader(memoryPool.Rent(buffSize), stream.ReadAsync);
            });
        }

        public static void UseBufferWriter(this IConfigurable configurable, int bufferSize)
        {
            const string SettingName = "MessageBuffer.BufferSize";

            configurable.AddSetting(SettingName, bufferSize);
            configurable.RegisterService(typeof(IBufferWriter<byte>), (p, s) =>
            {
                var memoryPool = (MemoryPool<byte>)p.GetService(typeof(MemoryPool<byte>));
                var transport = (ITransport)p.GetService(typeof(ITransport));
                var stream = transport.GetStream();
                var buffSize = (int)s[SettingName];

                return new ByteBufferWriter(memoryPool.Rent(buffSize), stream.WriteAsync);
            });
            configurable.RegisterService(typeof(FlushHandler), (p, s) =>
            {
                var writer = (ByteBufferWriter)p.GetService(typeof(IBufferWriter<byte>));
                return new FlushHandler(writer.FlushAsync);
            });
        }

        public static void UseMessageHandlerAggregate<T>(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IMessageHandlerRegistry<T>), (p, s) =>
            {
                return new MessageHandlerRegistry<T>();
            });

            configurable.RegisterService(typeof(Action<T>), (p, s) =>
            {
                var aggregate = (MessageHandlerRegistry<T>)p.GetService(typeof(IMessageHandlerRegistry<T>));

                return (Action<T>)aggregate.Handle;
            });
        }

        public static void UseBackgrounReceiver<T>(this IConfigurable configurable)
        {
            configurable.RegisterConfigurator((p, s) =>
            {
                var serializer = (ISerializer<T>)p.GetService(typeof(ISerializer<T>));
                var bufferReader = (IBufferReader)p.GetService(typeof(IBufferReader));
                var messageHandler = (Action<T>)p.GetService(typeof(Action<T>));
                var observer = new MessageBufferObserver<T>(serializer.Deserialize, bufferReader);
                observer.MessageDeserialized += messageHandler;
                var bgReceiver = new TransportWorker(observer.CheckBufferAsync);

                var transport = (ITransport)p.GetService(typeof(ITransport));
                transport.TransportEvent += bgReceiver.HandleTransportEventAsync;
            });
        }
    }
}
