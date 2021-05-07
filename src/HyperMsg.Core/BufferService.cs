using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;

namespace HyperMsg
{
    public class BufferService : MessagingService, IWriteToBufferCommandHandler
    {
        private readonly IBufferContext bufferContext;
        private readonly object transmittingBufferLock = new();
        private readonly object receivingBufferLock = new();

        public BufferService(IMessagingContext messagingContext, IBufferContext bufferContext) : base(messagingContext) => this.bufferContext = bufferContext;

        protected override IEnumerable<IDisposable> GetDefaultDisposables()
        {
            yield return this.RegisterReadFromBufferCommandHandler(HandleReadFromBufferCommand);
            yield return this.RegisterWriteToBufferCommandHandler(this);
            yield return this.RegisterBufferActionRequestHandler(HandleBufferActionRequest);
        }

        private void HandleBufferActionRequest(Action<IBuffer> bufferAction, BufferType bufferType)
        {
            switch (bufferType)
            {
                case BufferType.Transmitting:
                    HandleBufferActionRequest(bufferAction, bufferContext.TransmittingBuffer, transmittingBufferLock);
                    break;

                case BufferType.Receiving:
                    HandleBufferActionRequest(bufferAction, bufferContext.ReceivingBuffer, receivingBufferLock);
                    break;
            }
        }

        private void HandleBufferActionRequest(Action<IBuffer> bufferAction, IBuffer buffer, object bufferLock)
        {
            lock (bufferLock)
            {
                bufferAction.Invoke(buffer);
            }
        }

        private void HandleReadFromBufferCommand(BufferType bufferType, Func<ReadOnlySequence<byte>, int> bufferReader)
        {
            switch (bufferType)
            {
                case BufferType.Transmitting:
                    ReadFromBuffer(bufferType, bufferContext.TransmittingBuffer, bufferReader, transmittingBufferLock);
                    break;

                case BufferType.Receiving:
                    ReadFromBuffer(bufferType, bufferContext.ReceivingBuffer, bufferReader, receivingBufferLock);
                    break;
            }
        }

        private void ReadFromBuffer(BufferType bufferType, IBuffer buffer, Func<ReadOnlySequence<byte>, int> bufferReader, object bufferLock)
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

            OnBufferUpdated(bufferType);
        }

        public void WriteToBuffer<T>(BufferType bufferType, T message)
        {
            switch (bufferType)
            {
                case BufferType.Transmitting:
                    WriteToBuffer(bufferType, message, bufferContext.TransmittingBuffer, transmittingBufferLock);
                    break;

                case BufferType.Receiving:
                    WriteToBuffer(bufferType, message, bufferContext.ReceivingBuffer, receivingBufferLock);
                    break;
            }
        }

        private void WriteToBuffer<T>(BufferType bufferType, T message, IBuffer buffer, object bufferLock)
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
                        throw new NotSupportedException();
                        break;

                    default:
                        this.SendSerializationCommand(writer, message);
                        break;
                }
            }

            OnBufferUpdated(bufferType);
        }

        private void OnBufferUpdated(BufferType bufferType) => this.SendBufferUpdatedEventAsync(bufferType);
    }
}
