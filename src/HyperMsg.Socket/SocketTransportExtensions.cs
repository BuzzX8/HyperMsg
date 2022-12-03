using System.Net;

namespace HyperMsg.Socket;

public static class SocketTransportExtensions
{
    public static void DispatchConnectionRequest(this IDispatcher dispatcher, EndPoint endPoint)
        => dispatcher.Dispatch(new Connect(endPoint));

    public static void DispatchDisconnectionRequest(this IDispatcher dispatcher)
        => dispatcher.Dispatch(new Disconnect());

    public static void DispatchSendRequest(this IDispatcher dispatcher, Memory<byte> buffer)
        => dispatcher.Dispatch(new Send(buffer));
}
