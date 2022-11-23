using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Socket;

public class ConnectionService : Service
{
    private readonly System.Net.Sockets.Socket socket;
    private readonly SocketAsyncEventArgs asyncEventArgs;

    public ConnectionService(ITopic topic, System.Net.Sockets.Socket socket) : base(topic)
    {
        this.socket = socket;

        asyncEventArgs = new();
        asyncEventArgs.Completed += OperationCompleted;        
    }

    private void OperationCompleted(object? _, SocketAsyncEventArgs eventArgs) 
    {
        switch (eventArgs.LastOperation)
        {
            case SocketAsyncOperation.Connect: 
                Dispatch(new ConnectResult(eventArgs.RemoteEndPoint, eventArgs.SocketError));
                break;

            case SocketAsyncOperation.Disconnect:
                Dispatch(new DisconnectResult(eventArgs.SocketError));
                break;
        }
    }

    protected override void RegisterHandlers(IRegistry registry)
    {
        registry.Register<Connect>(Connect);
        registry.Register<Disconnect>(Disconnect);
    }
    
    protected override void UnregisterHandlers(IRegistry registry)
    {
        registry.Unregister<Connect>(Connect);
        registry.Unregister<Disconnect>(Disconnect);
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
            Dispatch(new DisconnectResult(SocketError.NotConnected));
            return;
        }

        if (!socket.DisconnectAsync(asyncEventArgs))
        {
            OperationCompleted(socket, asyncEventArgs);
        }
    }

    public void Dispose()
    {
        base.Dispose();
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
