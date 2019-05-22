namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void UseTransciever<T>(this IConfigurable configurable)
        {
            configurable.Configure(context => 
            {
                var receiver = (IReceiver<T>)context.GetService(typeof(IReceiver<T>));
                var sender = (ISender<T>)context.GetService(typeof(ISender<T>));
                var transciever = new MessageTransceiver<T>(receiver, sender);

                context.RegisterService(typeof(ITransceiver<T, T>), transciever);
            });
        }

        public static void UseMessageBuffer<T>(this IConfigurable configurable, int bufferSize)
        {
            const string SettingName = "MessageBuffer.BufferSize";

            configurable.AddSetting(SettingName, bufferSize);
            configurable.Configure(context =>
            {
                var serializer = (ISerializer<T>)context.GetService(typeof(ISerializer<T>));
                var stream = (IStream)context.GetService(typeof(IStream));
                var buffSize = (int)context.GetSetting(SettingName);
                var messageBuffer = new MessageBuffer<T>(serializer.Serialize, new byte[bufferSize], stream.WriteAsync);
            });
        }

        public static void UseMessageReceiver<T>(this IConfigurable configurable)
        {
            configurable.Configure(context =>
            {
                var reader = (IBufferReader)context.GetService(typeof(IBufferReader));
                var serializer = (ISerializer<T>)context.GetService(typeof(ISerializer<T>));
                var receiver = new MessageReceiver<T>(serializer.Deserialize, reader);

                context.RegisterService(typeof(IReceiver<T>), receiver);
            });
        }

        public static void UseBufferReader(this IConfigurable configurable, int bufferSize)
        {
            const string SettingName = "BufferReader.BufferSize";

            configurable.AddSetting(SettingName, bufferSize);
            configurable.Configure(context =>
            {
                var buffSize = (int)context.GetSetting(SettingName);
                var stream = (IStream)context.GetService(typeof(IStream));
                var reader = new BufferReader(new byte[buffSize], stream.ReadAsync);

                context.RegisterService(typeof(IBufferReader), reader);
            });
        }
    }
}
