using System.Net;

namespace HyperMsg.Sockets
{
    public static class TransceiverBuilderExtensions
    {
        public static void UseSockets<TSend, TReceive>(this IConfigurableBuilder<TSend, TReceive> transceiverBuilder, EndPoint endpoint)
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
