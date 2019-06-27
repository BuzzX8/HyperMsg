using System.Net;

namespace HyperMsg.Sockets
{
    public static class ConfigurableExtensions
    {
        public static void UseSockets(this IConfigurable configurable, EndPoint endpoint)
        {
            configurable.AddSetting(nameof(EndPoint), endpoint);
            configurable.RegisterService(typeof(ITransport), (p, s) =>
            {
                var registry = (IHandlerRegistry)p.GetService(typeof(IHandlerRegistry));
                var socket = new SocketProxy(SocketFactory.CreateTcpSocket(), (EndPoint)s[nameof(EndPoint)]);

                return new SocketTransport(socket);
            });
        }
    }
}
