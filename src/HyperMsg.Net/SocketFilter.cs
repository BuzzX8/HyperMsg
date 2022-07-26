using System.Net.Sockets;

namespace HyperMsg.Net;

internal class SocketFilter
{
    private readonly Socket socket;

    public SocketFilter(Socket socket) => this.socket = socket;

    public int Send(IBufferReader reader)
    {
        var bytes = reader.Read();
        var bytesSent = 0;

        bytes.ForEachSegment(memory => bytesSent += socket.Send(memory.Span));

        return bytesSent;
    }
}
