using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Socket;

public class ConnectionService
{
    private readonly IDispatcher dispatcher;
    private readonly IRegistry registry;

    private readonly System.Net.Sockets.Socket socket;
    private readonly SocketAsyncEventArgs asyncEventArgs;

    public ConnectionService(IDispatcher dispatcher, IRegistry registry, System.Net.Sockets.Socket socket)
    {
        this.dispatcher = dispatcher;
        this.registry = registry;
        this.socket = socket;

        asyncEventArgs = new();
        RegisterHandlers(registry);
    }

    private void RegisterHandlers(IRegistry registry)
    {

    }

    private void Connect(Connect connect)
    {
        asyncEventArgs.RemoteEndPoint = connect.EndPoint;
        socket.ConnectAsync(asyncEventArgs);
    }

    private void UnregisterHandlers(IRegistry registry)
    {

    }

    public void Dispose() => UnregisterHandlers(registry);
}

public record struct Connect(EndPoint EndPoint);

public record struct Disconnect;
