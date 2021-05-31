using HyperMsg.Messages;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BufferService : MessagingService
    {
        private readonly IBufferContext bufferContext;
        private readonly object transmittingBufferLock = new();
        private readonly object receivingBufferLock = new();

        public BufferService(IMessagingContext messagingContext, IBufferContext bufferContext) : base(messagingContext) => this.bufferContext = bufferContext;

        protected override IEnumerable<IDisposable> GetAutoDisposables()
        {
            yield return RegisterHandler<FlushBufferCommand>(HandleFlushBufferCommand);
            yield return RegisterHandler<SendToBufferCommand>(HandleSendToBuffer);
        }

        private void ReadFromBuffer(BufferType bufferType, BufferReader bufferReader)
        {
            (var buffer, var bufferLock) = GetBufferWithLock(bufferType);

            ReadFromBuffer(buffer, bufferReader, bufferLock);
        }

        private void ReadFromBuffer(IBuffer buffer, BufferReader bufferReader, object bufferLock)
        {
            lock (bufferLock)
            {
                var reading = buffer.Reader.Read();
                if (reading.Length == 0)
                {
                    return;
                }

                var bytesRead = bufferReader.Invoke(reading);

                if (bytesRead == 0)
                {
                    return;
                }

                buffer.Reader.Advance(bytesRead);
            }
        }

        private Task ReadFromBufferAsync(BufferType bufferType, AsyncBufferReader bufferReader)
        {
            ReadFromBuffer(bufferType, buffer => bufferReader.Invoke(buffer, default).Result);
            return Task.CompletedTask;
        }

        internal void WriteToBuffer<T>(BufferType bufferType, T message, bool flushBuffer)
        {
            (var buffer, var bufferLock) = GetBufferWithLock(bufferType);

            WriteToBuffer(bufferType, message, buffer, bufferLock, flushBuffer);
        }

        private (IBuffer buffer, object bufferLock) GetBufferWithLock(BufferType bufferType)
        {
            return bufferType switch
            {
                BufferType.Receiving => (bufferContext.ReceivingBuffer, receivingBufferLock),
                BufferType.Transmitting => (bufferContext.TransmittingBuffer, transmittingBufferLock),
                _ => throw new NotSupportedException($"Buffer type {bufferType} does not supported"),
            };
        }

        private void WriteToBuffer<T>(BufferType bufferType, T message, IBuffer buffer, object bufferLock, bool flushBuffer)
        {
            var writer = buffer.Writer;

            lock (bufferLock)
            {
                switch (message)
                {
                    case Memory<byte> memory:
                        writer.Write(memory.Span);
                        break;

                    case ReadOnlyMemory<byte> memory:
                        writer.Write(memory.Span);
                        break;

                    case ReadOnlySequence<byte> ros:
                        throw new NotSupportedException();
                        break;

                    case ArraySegment<byte> arraySegment:
                        writer.Write(arraySegment.AsSpan());
                        break;

                    case byte[] array:
                        writer.Write(array);
                        break;

                    case Stream stream:
                        WriteStream(writer, stream);
                        break;

                    default:
                        this.SendSerializeCommand(writer, message);
                        break;
                }
            }

            if (flushBuffer)
            {
                HandleFlushBufferCommand(new FlushBufferCommand(bufferType));
            }
        }

        private void WriteStream(IBufferWriter<byte> writer, Stream stream)
        {
            var buffer = writer.GetMemory();
            var bytesRead = stream.Read(buffer.Span);

            writer.Advance(bytesRead);
        }

        private void HandleFlushBufferCommand(FlushBufferCommand command) => SendAsync(new FlushBufferEvent(command.BufferType, reader => ReadFromBuffer(command.BufferType, reader), reader => ReadFromBufferAsync(command.BufferType, reader)), default);

        private void HandleSendToBuffer(SendToBufferCommand command)
        {
            command.WriteToBufferAction.Invoke(this);
        }
    }
}