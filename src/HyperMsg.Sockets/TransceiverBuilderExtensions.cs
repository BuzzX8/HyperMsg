using System.Net;

namespace HyperMsg.Sockets
{
    public static class TransceiverBuilderExtensions
    {
        public static void UseSockets(this ITransceiverBuilder<object, object> transceiverBuilder, EndPoint endPoint)
        {
            transceiverBuilder.Configure(context =>
            {
                var pipe = (IPipe)context.ServiceProvider.GetService(typeof(IPipe));
                var socket = SocketFactory.CreateTcpSocket();
                var worker = new SocketWorker(pipe, socket);
            });
        }
    }
}
