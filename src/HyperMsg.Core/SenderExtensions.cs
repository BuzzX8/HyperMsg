using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class SenderExtensions
    {
        #region Buffer extensions

        public static void SendActionRequestToReceiveBuffer(this ISender sender, Action<IBuffer> action) =>
            sender.SendActionRequestToBuffer(BufferType.Receive, action);

        public static Task SendActionRequestToReceiveBufferAsync(this ISender sender, AsyncAction<IBuffer> action, CancellationToken cancellationToken = default) =>
            sender.SendActionRequestToBufferAsync(BufferType.Receive, action, cancellationToken);
        
        public static void SendActionRequestToTransmitBuffer(this ISender sender, Action<IBuffer> action) =>
            sender.SendActionRequestToBuffer(BufferType.Transmit, action);

        public static Task SendActionRequestToTransmitBufferAsync(this ISender sender, AsyncAction<IBuffer> action, CancellationToken cancellationToken = default) =>
            sender.SendActionRequestToBufferAsync(BufferType.Transmit, action, cancellationToken);

        internal static void SendActionRequestToBuffer(this ISender sender, BufferType type, Action<IBuffer> action) =>
            sender.Send(new BufferActionRequest(type, action));
        
        internal static Task SendActionRequestToBufferAsync(this ISender sender, BufferType type, AsyncAction<IBuffer> action, CancellationToken cancellationToken = default) =>
            sender.SendAsync(new BufferAsyncActionRequest(type, action), cancellationToken);

        public static void SendInvokeReceiveBufferHandlersCommand(this ISender sender) =>
            sender.Send(new InvokeBufferHandlersCommand(BufferType.Receive));

        public static Task SendInvokeReceiveBufferHandlersCommandAsync(this ISender sender, CancellationToken cancellationToken = default) =>
            sender.SendAsync(new InvokeBufferHandlersCommand(BufferType.Receive), cancellationToken);
        
        public static void SendInvokeTransmitBufferHandlersCommand(this ISender sender) =>
            sender.Send(new InvokeBufferHandlersCommand(BufferType.Transmit));

        public static Task SendInvokeTransmitBufferHandlersCommandAsync(this ISender sender, CancellationToken cancellationToken = default) =>
            sender.SendAsync(new InvokeBufferHandlersCommand(BufferType.Transmit), cancellationToken);

        #endregion
    }
}