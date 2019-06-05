using System.Net;

namespace HyperMsg.Sockets
{
    public static class ConfigurableExtensions
    {
        public static void UseSockets(this IConfigurable configurable, EndPoint endpoint)
        {
            configurable.AddSetting(nameof(EndPoint), endpoint);
            configurable.RegisterService(typeof(IStream), (p, s) =>
            {
                var registry = (IHandlerRegistry)p.GetService(typeof(IHandlerRegistry));
                var socket = new SocketProxy(SocketFactory.CreateTcpSocket(), (EndPoint)s[nameof(EndPoint)]);                
                var transport = new SocketTransport(socket);
                registry.Register(transport);

                return transport;
            });
        }
    }
}
