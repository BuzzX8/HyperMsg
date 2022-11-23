using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Socket;

public class ConnectionService : IDisposable
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
        asyncEventArgs.Completed += OperationCompleted;
        RegisterHandlers(registry);
    }

    private void OperationCompleted(object? _, SocketAsyncEventArgs eventArgs) 
    {
        switch (eventArgs.LastOperation)
        {
            case SocketAsyncOperation.Connect: 
                dispatcher.Dispatch(new ConnectResult(eventArgs.RemoteEndPoint, eventArgs.SocketError));
                break;
        }
    }

    private void RegisterHandlers(IRegistry registry)
    {
        registry.Register<Connect>(Connect);
    }

    private void Connect(Connect connect)
    {
        asyncEventArgs.RemoteEndPoint = connect.EndPoint;

        if (!socket.ConnectAsync(asyncEventArgs))
        {
            OperationCompleted(socket, asyncEventArgs);
        }
    }

    private void UnregisterHandlers(IRegistry registry)
    {
        registry.Deregister<Connect>(Connect);
    }

    public void Dispose()
    {
        UnregisterHandlers(registry);
        asyncEventArgs.Completed -= OperationCompleted;
        asyncEventArgs.Dispose();

        if (socket.Connected)
        {
            socket.Shutdown(SocketShutdown.Both);
        }

        socket.Dispose();
    }
}

public record struct Connect(EndPoint EndPoint);

public record struct ConnectResult(EndPoint? EndPoint, SocketError Error);

public record struct Disconnect;

public record struct Disconnected;
