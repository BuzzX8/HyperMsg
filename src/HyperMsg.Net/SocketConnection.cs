using System.Net;
using System.Net.Sockets;

namespace HyperMsg.Net;

public class SocketConnection
{
    private readonly Socket socket;
    private readonly EndPoint endPoint;

    public bool IsOpen => socket.Connected;

    public ConnectionState State { get; private set; }

    private void ChangeState(ConnectionState state)
    {
        State = state;
        StateChanged?.Invoke(state);
    }

    public event Action<ConnectionState> StateChanged;
}
