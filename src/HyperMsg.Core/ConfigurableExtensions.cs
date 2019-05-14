using System;

namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void UseTransciever<T>(this IConfigurable configurable)
        {
            configurable.Configure(context => 
            {
                var serializer = (ISerializer<T>)context.GetService(typeof(ISerializer<T>));
                var stream = (IStream)context.GetService(typeof(IStream));
                var transciever = new MessageTransceiver<T>(serializer, new Memory<byte>(), new Memory<byte>(), stream);

                context.RegisterService(typeof(ITransceiver<T, T>), transciever);
            });
        }
    }
}
