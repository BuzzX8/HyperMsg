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
                var transport = new SocketTransport(socket);
                context.Services.Add(ServiceDescriptor.Describe(typeof(IStream), transport));
                context.Services.Add(ServiceDescriptor.Describe(typeof(IHandler<TransportCommands>), transport));
            });
        }
    }
}
