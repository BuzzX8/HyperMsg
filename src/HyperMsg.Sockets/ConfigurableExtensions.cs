using System.Net;

namespace HyperMsg.Sockets
{
    public static class ConfigurableExtensions
    {
        public static void UseSockets(this IConfigurable configurable, EndPoint endpoint)
        {
            configurable.AddSetting(nameof(EndPoint), endpoint);
            configurable.Configure(AddSocketServices);
        }

        private static void AddSocketServices(IConfigurationContext context)
        {
            var socket = new SocketProxy(SocketFactory.CreateTcpSocket(), (EndPoint)context.GetSetting(nameof(EndPoint)));
            var transport = new SocketTransport(socket);
            context.RegisterService(typeof(IStream), transport);
            context.RegisterService(typeof(IHandler<TransportCommands>), transport);
        }
    }
}
