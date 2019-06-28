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
                var ep = (EndPoint)s[nameof(EndPoint)];
                var socket = new SocketProxy(SocketFactory.CreateTcpSocket(), ep);

                return new SocketTransport(socket);
            });
        }
    }
}
