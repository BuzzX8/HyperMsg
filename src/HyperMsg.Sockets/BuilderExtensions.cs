using System.Net;

namespace HyperMsg.Sockets
{
    public static class BuilderExtensions
    {
        public static void UseSockets<T>(this IConfigurableBuilder<T> transceiverBuilder, EndPoint endpoint)
        {
            transceiverBuilder.Configure(context =>
            {
                var socket = new SocketProxy(SocketFactory.CreateTcpSocket(), endpoint);
                var stream = new SocketStream(socket);
                context.Services.Add(ServiceDescriptor.Describe(typeof(IStream), stream));
            });
        }
    }
}
