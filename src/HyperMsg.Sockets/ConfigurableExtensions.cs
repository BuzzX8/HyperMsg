using System.Net;

namespace HyperMsg.Sockets
{
    public static class ConfigurableExtensions
    {
        public static void UseSockets<T>(this IConfigurable configurable, EndPoint endpoint)
        {
            configurable.Configure(context =>
            {
                var socket = new SocketProxy(SocketFactory.CreateTcpSocket(), endpoint);
                var stream = new SocketStream(socket);
                context.Services.Add(ServiceDescriptor.Describe(typeof(IStream), stream));
            });
        }
    }
}
