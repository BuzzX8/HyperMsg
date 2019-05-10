using System;

namespace HyperMsg
{
    public static class ConfigurableExtensions
    {
        public static void UseTransciever<T>(this IConfigurable configurable)
        {
            configurable.Configure((c, s) => 
            {
                var service = ServiceDescriptor.Describe(typeof(ITransceiver<T, T>), p =>
                {
                    var serializer = (ISerializer<T>)p.GetService(typeof(ISerializer<T>));
                    var stream = (IStream)p.GetService(typeof(IStream));
                    var transciever = new MessageTransceiver<T>(serializer, new Memory<byte>(), new Memory<byte>(), stream);
                    return transciever;
                });
                c.Services.Add(service);
            }, null);
        }
    }
}
