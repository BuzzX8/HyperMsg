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

            case SocketAsyncOperation.Disconnect:
                dispatcher.Dispatch(new DisconnectResult(eventArgs.SocketError));
                break;
        }
    }

    private void RegisterHandlers(IRegistry registry)
    {
        registry.Register<Connect>(Connect);
        registry.Register<Disconnect>(Disconnect);
    }

    private void Connect(Connect connect)
    {
        asyncEventArgs.RemoteEndPoint = connect.RemoteEndPoint;

        if (!socket.ConnectAsync(asyncEventArgs))
        {
            OperationCompleted(socket, asyncEventArgs);
        }
    }

    private void Disconnect(Disconnect _)
    {
        if (!socket.Connected)
        {
            dispatcher.Dispatch(new DisconnectResult(SocketError.NotConnected));
            return;
        }

        if (!socket.DisconnectAsync(asyncEventArgs))
        {
            OperationCompleted(socket, asyncEventArgs);
        }
    }

    private void UnregisterHandlers(IRegistry registry)
    {
        registry.Deregister<Connect>(Connect);
        registry.Deregister<Disconnect>(Disconnect);
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

public record struct Connect(EndPoint RemoteEndPoint);

public record struct ConnectResult(EndPoint? RemoteEndPoint, SocketError Error);

public record struct Disconnect;

public record struct DisconnectResult(SocketError Error);
