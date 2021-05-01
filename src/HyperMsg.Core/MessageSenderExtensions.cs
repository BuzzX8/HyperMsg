﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class MessageSenderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void SendMessageReceivedEvent<T>(this IMessageSender messageSender, T message) => messageSender.Send(new MessageReceivedEvent<T>(message));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendMessageReceivedEventAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken = default) => messageSender.SendAsync(new MessageReceivedEvent<T>(message), cancellationToken);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        public static void SendTransmitMessageCommand<T>(this IMessageSender messageSender, T message) => messageSender.Send(new TransmitMessageCommand<T>(message));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Type of message.</typeparam>
        /// <param name="messageSender">Message sender.</param>
        /// <param name="message">Message to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public static Task SendTransmitMessageCommandAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken = default) => messageSender.SendAsync(new TransmitMessageCommand<T>(message), cancellationToken);

        public static async Task SendTransmitBufferDataCommandAsync(this IMessageSender messageSender, IBuffer transmittingBuffer, CancellationToken cancellationToken = default)
        {
            var reader = transmittingBuffer.Reader;
            var buffer = reader.Read();

            if (buffer.Length == 0)
            {
                return;
            }

            if (buffer.IsSingleSegment)
            {
                await messageSender.SendTransmitMessageCommandAsync(buffer.First, cancellationToken);
                reader.Advance((int)buffer.Length);
                return;
            }

            var enumerator = buffer.GetEnumerator();

            while (enumerator.MoveNext())
            {
                await messageSender.SendTransmitMessageCommandAsync(enumerator.Current, cancellationToken);
            }
        }

        public static void SendReceivingBufferUpdatedEvent(this IMessageSender messageSender, IBuffer receivingBuffer) => messageSender.SendMessageReceivedEvent(receivingBuffer);

        public static Task SendReceivingBufferUpdatedEventAsync(this IMessageSender messageSender, IBuffer receivingBuffer, CancellationToken cancellationToken = default) =>
            messageSender.SendMessageReceivedEventAsync(receivingBuffer, cancellationToken);

        public static void SendSerializationCommand<T>(this IMessageSender messageSender, IBufferWriter<byte> bufferWriter, T message) =>
            messageSender.Send(new SerializationCommand<T>(bufferWriter, message));

        public static Task SendSerializationCommandAsync<T>(this IMessageSender messageSender, IBufferWriter<byte> bufferWriter, T message, CancellationToken cancellationToken = default) =>
            messageSender.SendAsync(new SerializationCommand<T>(bufferWriter, message), cancellationToken);

        public static void SendBufferActionRequest(this IMessageSender messageSender, Action<IBuffer> action, BufferType bufferType) => messageSender.Send(new BufferActionRequest(bufferType, action));

        public static Task SendBufferActionRequestAsync(this IMessageSender messageSender, Action<IBuffer> action, BufferType bufferType, CancellationToken cancellationToken = default) => 
            messageSender.SendAsync(new BufferActionRequest(bufferType, action), cancellationToken);

        public static void SendReadFromBufferCommand(this IMessageSender messageSender, BufferType bufferType, Func<ReadOnlySequence<byte>, int> bufferReader) => 
            messageSender.Send(new ReadFromBufferCommand(bufferType, bufferReader));

        public static Task SendReadFromBufferCommandAsync(this IMessageSender messageSender, BufferType bufferType, Func<ReadOnlySequence<byte>, int> bufferReader, CancellationToken cancellationToken = default) => 
            messageSender.SendAsync(new ReadFromBufferCommand(bufferType, bufferReader), cancellationToken);

        public static void SendWriteToBufferCommand<T>(this IMessageSender messageSender, BufferType bufferType, T message) => 
            messageSender.Send<Action<IWriteToBufferCommandHandler>>(handler => handler.WriteToBuffer(bufferType, message));

        public static Task SendWriteToBufferCommandAsync<T>(this IMessageSender messageSender, BufferType bufferType, T message, CancellationToken cancellationToken = default) => 
            messageSender.SendAsync<Action<IWriteToBufferCommandHandler>>(handler => handler.WriteToBufferAsync(bufferType, message, cancellationToken), cancellationToken);

        public static void SendBufferUpdatedEvent(this IMessageSender messageSender, BufferType bufferType, IBuffer buffer) => messageSender.Send(new BufferUpdatedEvent(bufferType, buffer));

        public static Task SendBufferUpdatedEventAsync(this IMessageSender messageSender, BufferType bufferType, IBuffer buffer, CancellationToken cancellationToken = default) => 
            messageSender.SendAsync(new BufferUpdatedEvent(bufferType, buffer), cancellationToken);

        public static IServiceScope SendCreateServiceScopeRequest(this IMessageSender messageSender, Action<IServiceCollection> serviceConfigurator)
        {
            var command = new StartServiceScopeRequest { ServiceConfigurator = serviceConfigurator };
            messageSender.Send(command);
            
            return command.ServiceScope;
        }

        public static async Task<IServiceScope> SendCreateServiceScopeRequestAsync(this IMessageSender messageSender, Action<IServiceCollection> serviceConfigurator, CancellationToken cancellationToken = default)
        {
            var command = new StartServiceScopeRequest { ServiceConfigurator = serviceConfigurator };
            await messageSender.SendAsync(command, cancellationToken);

            return command.ServiceScope;
        }
    }
}
