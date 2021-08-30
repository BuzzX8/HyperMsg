using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class SenderExtensions
    {
        #region Basic extensions

        /// <param name="message">Message to send.</param>
        public static void SendMessage<THeader, TBody>(this ISender sender, THeader header, TBody body) =>
            sender.Send(new Message<THeader, TBody>(header, body));

        public static Task SendMessageAsync<THeader, TBody>(this ISender sender, THeader header, TBody body, CancellationToken cancellationToken = default) =>
            sender.SendAsync(new Message<THeader, TBody>(header, body), cancellationToken);

        public static void SendCommand<T>(this ISender sender, T command) =>
            sender.SendMessage(BasicMessageType.Command, command);

        public static Task SendCommandAsync<T>(this ISender sender, T command, CancellationToken cancellationToken = default) =>
            sender.SendMessageAsync(BasicMessageType.Command, command, cancellationToken);

        public static void SendEvent<T>(this ISender sender, T @event) =>
            sender.SendMessage(BasicMessageType.Event, @event);

        public static Task SendEventAsync<T>(this ISender sender, T @event, CancellationToken cancellationToken = default) =>
            sender.SendMessageAsync(BasicMessageType.Event, @event, cancellationToken);

        #endregion

        #region Transfering extensions

        public static void SendTransmitCommand<T>(this ISender sender, T data) =>
            sender.SendCommand(new Message<BasicMessageType, T>(BasicMessageType.Transmit, data));
        
        public static Task SendTransmitCommandAsync<T>(this ISender sender, T data, CancellationToken cancellationToken = default) =>
            sender.SendCommandAsync(new Message<BasicMessageType, T>(BasicMessageType.Transmit, data));

        public static void SendReceiveEvent<T>(this ISender sender, T data) =>
            sender.SendEvent(new Message<BasicMessageType, T>(BasicMessageType.Receive, data));
        
        public static Task SendReceiveEventAsync<T>(this ISender sender, T data, CancellationToken cancellationToken = default) =>
            sender.SendEventAsync(new Message<BasicMessageType, T>(BasicMessageType.Receive, data), cancellationToken);

        #endregion

        #region Buffer extensions

        public static void SendActionRequestToReceiveBuffer(this ISender sender, Action<IBuffer> action) =>
            sender.SendActionRequestToBuffer(BasicMessageType.Receive, action);

        public static Task SendActionRequestToReceiveBufferAsync(this ISender sender, AsyncAction<IBuffer> action, CancellationToken cancellationToken = default) =>
            sender.SendActionRequestToBufferAsync(BasicMessageType.Receive, action, cancellationToken);
        
        public static void SendActionRequestToTransmitBuffer(this ISender sender, Action<IBuffer> action) =>
            sender.SendActionRequestToBuffer(BasicMessageType.Transmit, action);

        public static Task SendActionRequestToTransmitBufferAsync(this ISender sender, AsyncAction<IBuffer> action, CancellationToken cancellationToken = default) =>
            sender.SendActionRequestToBufferAsync(BasicMessageType.Transmit, action, cancellationToken);

        internal static void SendActionRequestToBuffer(this ISender sender, BasicMessageType type, Action<IBuffer> action) =>
            sender.SendMessage(type, action);
        
        internal static Task SendActionRequestToBufferAsync(this ISender sender, BasicMessageType type, AsyncAction<IBuffer> action, CancellationToken cancellationToken = default) =>
            sender.SendMessageAsync(type, action);

        #endregion
    }
}