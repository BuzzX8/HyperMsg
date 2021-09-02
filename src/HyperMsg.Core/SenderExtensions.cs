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

        public static void SendHandleReceiveBufferCommand(this ISender sender) =>
            sender.Send(new HandleBufferRequest(BufferType.Receive));

        public static Task SendHandleReceiveBufferCommandAsync(this ISender sender, CancellationToken cancellationToken = default) =>
            sender.SendAsync(new HandleBufferRequest(BufferType.Receive), cancellationToken);
        
        public static void SendHandleTransmitBufferCommand(this ISender sender) =>
            sender.Send(new HandleBufferRequest(BufferType.Transmit));

        public static Task SendHandleTransmitBufferCommandAsync(this ISender sender, CancellationToken cancellationToken = default) =>
            sender.SendAsync(new HandleBufferRequest(BufferType.Transmit), cancellationToken);

        internal static void SendHandleBufferCommand(this ISender sender, BufferType type) =>
            sender.Send(new HandleBufferRequest(type));

        internal static Task SendHandleBufferCommandAsync(this ISender sender, BufferType type, CancellationToken cancellationToken) =>
            sender.SendAsync(new HandleBufferRequest(type), cancellationToken);

        #endregion
    }
}