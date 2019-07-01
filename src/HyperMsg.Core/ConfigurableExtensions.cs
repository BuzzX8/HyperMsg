namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void UseCoreServices<T>(this IConfigurable configurable, int inputBufferSize, int outputBufferSize)
        {
            configurable.UseMessageHandlerAggregate<T>();
            configurable.UseBackgrounReceiver<T>();
            configurable.UseBufferReader(inputBufferSize);
            configurable.UseMessageBuffer<T>(outputBufferSize);
        }

        public static void UseMessageBuffer<T>(this IConfigurable configurable, int bufferSize)
        {
            const string SettingName = "MessageBuffer.BufferSize";

            configurable.AddSetting(SettingName, bufferSize);
            configurable.RegisterService(new[] { typeof(IMessageSender<T>), typeof(IMessageBuffer<T>) }, (p, s) =>
            {
                var serializer = (ISerializer<T>)p.GetService(typeof(ISerializer<T>));
                var transport = (ITransport)p.GetService(typeof(ITransport));
                var stream = transport.GetStream();
                var buffSize = (int)s[SettingName];
                return new MessageBuffer<T>(serializer.Serialize, new byte[bufferSize], stream.WriteAsync);
            });
        }

        public static void UseBufferReader(this IConfigurable configurable, int bufferSize)
        {
            const string SettingName = "BufferReader.BufferSize";

            configurable.AddSetting(SettingName, bufferSize);
            configurable.RegisterService(typeof(IBufferReader), (p, s) =>
            {
                var buffSize = (int)s[SettingName];
                var transport = (ITransport)p.GetService(typeof(ITransport));
                var stream = transport.GetStream();
                return new BufferReader(new byte[buffSize], stream.ReadAsync);
            });
        }

        public static void UseMessageHandlerAggregate<T>(this IConfigurable configurable)
        {
            configurable.RegisterService(typeof(IMessageHandlerRegistry<T>), (p, s) =>
            {
                return new MessageHandlerAggregate<T>();
            });

            configurable.RegisterService(typeof(AsyncHandler<T>), (p, s) =>
            {
                var aggregate = (MessageHandlerAggregate<T>)p.GetService(typeof(IMessageHandlerRegistry<T>));

                return (AsyncHandler<T>)aggregate.HandleAsync;
            });
        }

        public static void UseBackgrounReceiver<T>(this IConfigurable configurable)
        {
            configurable.RegisterConfigurator((p, s) =>
            {
                var serializer = (ISerializer<T>)p.GetService(typeof(ISerializer<T>));
                var bufferReader = (IBufferReader)p.GetService(typeof(IBufferReader));
                var messageHandler = (AsyncHandler<T>)p.GetService(typeof(AsyncHandler<T>));
                //var bgReceiver = new BackgroundReceiver<T>(serializer.Deserialize, bufferReader, messageHandler);

                var transport = (ITransport)p.GetService(typeof(ITransport));
                //transport.TransportEvent += bgReceiver.HandleTransportEventAsync;
            });
        }
    }
}
