using System.Net;

namespace HyperMsg.Socket;

public static class SocketTransportExtensions
{
    public static void DispatchConnectRequest(this IDispatcher dispatcher, EndPoint endPoint)
        => dispatcher.Dispatch(new ConnectRequest(endPoint));

    public static void DispatchDisconnectRequest(this IDispatcher dispatcher)
        => dispatcher.Dispatch(new DisconnectRequest());

    public static void DispatchSendRequest(this IDispatcher dispatcher, Memory<byte> buffer)
        => dispatcher.Dispatch(new SendRequest(buffer));

    public static void DispatchReceiveRequest(this IDispatcher dispatcher, Memory<byte> buffer) 
        => dispatcher.Dispatch(new ReceiveRequest(buffer));

    public static void DispatchReceiveInBufferRequest(this IDispatcher dispatcher)
        => dispatcher.Dispatch(new ReceiveInBufferRequest());
}
