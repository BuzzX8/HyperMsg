using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Net;

public class SocketBinding
{
    private readonly Socket socket;
    private readonly EndPoint endPoint;
    private readonly int backlog;

    private readonly SocketAsyncEventArgs asyncEventArgs;

    internal SocketBinding(Socket socket, EndPoint endPoint, int backlog = 1)
    {
        this.socket = socket;
        this.endPoint = endPoint;
        this.backlog = backlog;

        asyncEventArgs = new();
        asyncEventArgs.Completed += OnSocketAccepted;
    }

    public bool IsBound => socket.IsBound;

    public void AcceptSocket()
    {
        if (!socket.AcceptAsync(asyncEventArgs))
        {
            OnSocketAccepted(this, asyncEventArgs);
        }
    }

    public void Bind()
    {
        if (IsBound)
        {
            return;
        }

        socket.Bind(endPoint);
        socket.Listen(backlog);
    }

    private void OnSocketAccepted(object? sender, SocketAsyncEventArgs asyncEventArgs)
    {
        SocketAccepted?.Invoke(asyncEventArgs.AcceptSocket);
    }

    public event Action<Socket?>? SocketAccepted;
}
