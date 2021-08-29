using HyperMsg.Messages;
using System;
using System.Buffers;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class HandlersRegistryExtensions
    {
        #region Basic extensions
        
        public static IDisposable RegisterMessageHandler<THeader, TBody>(this IHandlersRegistry handlersRegistry, Action<THeader, TBody> messageHandler) =>
            handlersRegistry.RegisterHandler<Message<THeader, TBody>>(m => messageHandler.Invoke(m.Header, m.Body));

        public static IDisposable RegisterMessageHandler<THeader, TBody>(this IHandlersRegistry handlersRegistry, AsyncAction<THeader, TBody> messageHandler) =>
            handlersRegistry.RegisterHandler<Message<THeader, TBody>>((m, t) => messageHandler.Invoke(m.Header, m.Body, t));

        public static IDisposable RegisterMessageHandler<THeader, TBody>(this IHandlersRegistry handlersRegistry, THeader header, Action<TBody> messageHandler) =>
            handlersRegistry.RegisterMessageHandler<THeader, TBody>((h, b) =>
            {
                if (!Equals(header, h))
                {
                    return;
                }

                messageHandler.Invoke(b);
            });

        public static IDisposable RegisterMessageHandler<THeader, TBody>(this IHandlersRegistry handlersRegistry, THeader header, AsyncAction<TBody> messageHandler) =>
            handlersRegistry.RegisterMessageHandler<THeader, TBody>((h, b, t) =>
            {
                if (!Equals(header, h))
                {
                    return Task.CompletedTask;
                }

                return messageHandler.Invoke(b, t);
            });

        public static IDisposable RegisterCommandHandler<T>(this IHandlersRegistry handlersRegistry, Action<T> commandHandler) =>
            handlersRegistry.RegisterMessageHandler(BasicMessageType.Command, commandHandler);

        public static IDisposable RegisterCommandHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<T> commandHandler) =>
            handlersRegistry.RegisterMessageHandler(BasicMessageType.Command, commandHandler);

        public static IDisposable RegisterEventHandler<T>(this IHandlersRegistry handlersRegistry, Action<T> eventHandler) =>
            handlersRegistry.RegisterMessageHandler(BasicMessageType.Event, eventHandler);

        public static IDisposable RegisterEventHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<T> eventHandler) =>
            handlersRegistry.RegisterMessageHandler(BasicMessageType.Event, eventHandler);

        #endregion

        #region Transfering extensions

        public static IDisposable RegisterTransmitCommandHandler<T>(this IHandlersRegistry handlersRegistry, Action<T> commandHandler) =>
            handlersRegistry.RegisterCommandHandler<Message<BasicMessageType, T>>(message =>
            {
                if (!Equals(message.Header, BasicMessageType.Transmit))
                {
                    return;
                }

                commandHandler.Invoke(message.Body);
            });

        public static IDisposable RegisterTransmitCommandHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<T> commandHandler) =>
            handlersRegistry.RegisterCommandHandler<Message<BasicMessageType, T>>((message, token) =>
            {
                if (!Equals(message.Header, BasicMessageType.Transmit))
                {
                    return Task.CompletedTask;
                }

                return commandHandler.Invoke(message.Body, token);
            });

        public static IDisposable RegisterReceiveEventHandler<T>(this IHandlersRegistry handlersRegistry, Action<T> commandHandler) =>
            handlersRegistry.RegisterEventHandler<Message<BasicMessageType, T>>(message =>
            {
                if (!Equals(message.Header, BasicMessageType.Receive))
                {
                    return;
                }

                commandHandler.Invoke(message.Body);
            });

        public static IDisposable RegisterReceiveEventHandler<T>(this IHandlersRegistry handlersRegistry, AsyncAction<T> commandHandler) =>
            handlersRegistry.RegisterEventHandler<Message<BasicMessageType, T>>((message, token) =>
            {
                if (!Equals(message.Header, BasicMessageType.Receive))
                {
                    return Task.CompletedTask;
                }

                return commandHandler.Invoke(message.Body, token);
            });

        #endregion

        #region Buffer extensions

        
        #endregion

        #region Filter extensions

        public static IDisposable RegisterFilter<T>(this IHandlersRegistry handlersRegistry, Action<ISender, T> filterHandler) =>
            handlersRegistry.RegisterMessageHandler(filterHandler);
        
        public static IDisposable RegisterFilter<T>(this IHandlersRegistry handlersRegistry, AsyncAction<ISender, T> filterHandler) =>
            handlersRegistry.RegisterMessageHandler(filterHandler);

        #endregion

        #region Serialization extensions

        

        #endregion
    }
}