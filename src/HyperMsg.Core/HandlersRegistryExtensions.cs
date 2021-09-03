﻿using System;
using System.Threading.Tasks;
using HyperMsg.Messages;

namespace HyperMsg
{
    public static class HandlersRegistryExtensions
    {
        #region Buffer extensions

        public static IDisposable RegisterTransmitBufferHandler(this IHandlersRegistry handlersRegistry, Action<IBuffer> bufferHandler) =>
            handlersRegistry.RegisterBufferHandler(BufferType.Transmit, bufferHandler);
        
        public static IDisposable RegisterTransmitBufferHandler(this IHandlersRegistry handlersRegistry, AsyncAction<IBuffer> bufferHandler) =>
            handlersRegistry.RegisterBufferHandler(BufferType.Transmit, bufferHandler);
        
        public static IDisposable RegisterReceiveBufferHandler(this IHandlersRegistry handlersRegistry, Action<IBuffer> bufferHandler) =>
            handlersRegistry.RegisterBufferHandler(BufferType.Receive, bufferHandler);
        
        public static IDisposable RegisterReceiveBufferHandler(this IHandlersRegistry handlersRegistry, AsyncAction<IBuffer> bufferHandler) =>
            handlersRegistry.RegisterBufferHandler(BufferType.Receive, bufferHandler);

        internal static IDisposable RegisterBufferHandler(this IHandlersRegistry handlersRegistry, BufferType bufferType, Action<IBuffer> bufferHandler) =>
            handlersRegistry.RegisterHandler<HandleBufferCommand>(command => 
            {
                if (command.BufferType != bufferType)
                {
                    return;
                }

                bufferHandler.Invoke(command.Buffer);
            });
        
        internal static IDisposable RegisterBufferHandler(this IHandlersRegistry handlersRegistry, BufferType bufferType, AsyncAction<IBuffer> bufferHandler) =>
            handlersRegistry.RegisterHandler<HandleBufferCommand>((command, token) => 
            {
                if (command.BufferType != bufferType)
                {
                    return Task.CompletedTask;
                }

                return bufferHandler.Invoke(command.Buffer, token);
            });

        #endregion

        #region Serialization extensions

        public static IDisposable RegisterSerializer<T>(this IHandlersRegistry handlersRegistry, Action<IBufferWriter, T> serializer) =>
             handlersRegistry.RegisterHandler<SerializationCommand<T>>(command =>
             {
                 command.Sender.SendActionRequestToTransmitBuffer(buffer => serializer.Invoke(buffer.Writer, command.Message));
                 command.Sender.SendInvokeTransmitBufferHandlersCommand();
             });

        #endregion
    }
}