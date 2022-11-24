using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Socket;

public class ConnectionService : Service
{
    private readonly SocketHolder socketHolder;
    private readonly SocketAsyncEventArgs asyncEventArgs;

    public ConnectionService(ITopic topic, SocketHolder socketHolder) : base(topic)
    {
        this.socketHolder = socketHolder;

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

        if (!socketHolder.Socket.ConnectAsync(asyncEventArgs))
        {
            OperationCompleted(socketHolder, asyncEventArgs);
        }
    }

    private void Disconnect(Disconnect _)
    {
        if (!socketHolder.Socket.Connected)
        {
            Dispatch(new DisconnectResult(SocketError.NotConnected));
            return;
        }

        if (!socketHolder.Socket.DisconnectAsync(asyncEventArgs))
        {
            OperationCompleted(socketHolder, asyncEventArgs);
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        asyncEventArgs.Completed -= OperationCompleted;
        asyncEventArgs.Dispose();    
    }
}

public record struct Connect(EndPoint RemoteEndPoint);

public record struct ConnectResult(EndPoint? RemoteEndPoint, SocketError Error);

public record struct Disconnect;

public record struct DisconnectResult(SocketError Error);
