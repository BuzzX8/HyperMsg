using System.Net;

namespace HyperMsg.Sockets
{
    public static class ConfigurableExtensions
    {
        public static void UseSockets(this IConfigurable configurable, EndPoint endpoint)
        {
            configurable.AddSetting(nameof(EndPoint), endpoint);
            configurable.AddService(typeof(IStream), (p, s) =>
            {
                var repository = (IHandlerRepository)p.GetService(typeof(IHandlerRepository));
                var socket = new SocketProxy(SocketFactory.CreateTcpSocket(), (EndPoint)s[nameof(EndPoint)]);                
                var transport = new SocketTransport(socket);
                repository.AddHandler(transport);

                return transport;
            });
        }
    }
}
