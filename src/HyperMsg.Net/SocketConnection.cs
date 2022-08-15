using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Net;

public class SocketConnection : IConnectionStateObserver
{
    private readonly Socket socket;
    private readonly EndPoint endPoint;
    private readonly object sync;

    internal SocketConnection(Socket socket, EndPoint endPoint)
    {
        this.endPoint = endPoint;
        this.socket = socket;
        State = socket.Connected ? ConnectionState.Opened : ConnectionState.Closed;
        sync = new();
    }

    public bool IsOpen => socket.Connected;

    public ConnectionState State { get; private set; }

    public void Open()
    {
        lock (sync)
        {

            if (IsOpen)
            {
                return;
            }

            ChangeState(ConnectionState.Opening);
            socket.Connect(endPoint);
            ChangeState(ConnectionState.Opened);
        }
    }

    private void ChangeState(ConnectionState state)
    {
        State = state;
        StateChanged?.Invoke(state);
    }

    public static SocketConnection CreateTcpConnection(IPAddress address, int port) => CreateTcpConnection(new IPEndPoint(address, port));

    public static SocketConnection CreateTcpConnection(EndPoint endPoint)
    {
        var socket = new Socket(SocketType.Stream, ProtocolType.Tcp);
        return new(socket, endPoint);
    }

    public event Action<ConnectionState>? StateChanged;
}

public interface IConnectionStateObserver
{
    event Action<ConnectionState> StateChanged;
}