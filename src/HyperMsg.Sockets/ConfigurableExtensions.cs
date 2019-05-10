using System.Net;

namespace HyperMsg.Sockets
{
    public static class ConfigurableExtensions
    {
        public static void UseSockets<T>(this IConfigurable configurable, EndPoint endpoint)
        {
            configurable.Configure(AddSocketServices, endpoint);
        }

        private static void AddSocketServices(Configuration configuration, object settings)
        {
            var socket = new SocketProxy(SocketFactory.CreateTcpSocket(), (EndPoint)settings);
            var transport = new SocketTransport(socket);
            configuration.Services.Add(ServiceDescriptor.Describe(typeof(IStream), transport));
            configuration.Services.Add(ServiceDescriptor.Describe(typeof(IHandler<TransportCommands>), transport));
        }
    }
}
