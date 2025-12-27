using HyperMsg.Buffers;
using System.Net.Sockets;

namespace HyperMsg.Transport.Sockets;

/// <summary>
/// Provides a socket-based transport implementation for the HyperMsg framework.
/// </summary>
/// <remarks>
/// This class manages the lifecycle and data transmission of a <see cref="Socket"/> connection, including state management and event notification.
/// </remarks>
public class SocketTransport : ITransportContext, IDisposable
{
    private readonly SocketConnection _connection;
    private readonly SocketChannel _channel;
    private readonly IBufferingContext? bufferingContext;

    public SocketTransport(ISocket socket, IBufferingContext? bufferingContext=null)
    {
        _connection = new(socket);
        _channel = new(socket);

        this.bufferingContext = bufferingContext;
        if (bufferingContext is not null)
        {
            bufferingContext.InputBufferUpstreamUpdateRequested += BufferingContext_InputBufferUpstreamUpdateRequested;
            bufferingContext.OutputBufferDownstreamUpdateRequested += BufferingContext_OutputBufferDownstreamUpdateRequested;
        }
    }

    private async ValueTask BufferingContext_OutputBufferDownstreamUpdateRequested(IBuffer buffer, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private ValueTask BufferingContext_InputBufferUpstreamUpdateRequested(IBuffer buffer, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    #region ITransportContext Members

    /// <inheritdoc/>
    public IConnection Connection => _connection;

    public IChannel Channel => _channel;

    #endregion

    public void Dispose()
    {
        _connection.Dispose();
    }
}