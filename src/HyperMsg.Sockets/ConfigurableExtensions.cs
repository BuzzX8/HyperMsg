using System.Net;

namespace HyperMsg.Sockets
{
    public static class ConfigurableExtensions
    {
        public static void UseSockets(this IConfigurable configurable, EndPoint endpoint)
        {
            configurable.AddSetting(nameof(EndPoint), endpoint);
            configurable.AddService(new[] { typeof(IStream), typeof(IHandler<TransportCommands>) }, (p, s) =>
            {
                var socket = new SocketProxy(SocketFactory.CreateTcpSocket(), (EndPoint)s[nameof(EndPoint)]);
                return new SocketTransport(socket);
            });
        }
    }
}
