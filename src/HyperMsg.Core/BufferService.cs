using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
                    ReadFromBuffer(bufferContext.TransmittingBuffer, bufferReader, transmittingBufferLock);
                    break;

                case BufferType.Receiving:
                    ReadFromBuffer(bufferContext.ReceivingBuffer, bufferReader, receivingBufferLock);
                    break;
            }
        }

        private void ReadFromBuffer(IBuffer buffer, Func<ReadOnlySequence<byte>, int> bufferReader, object bufferLock)
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

            this.SendBufferUpdatedEvent(bufferType, buffer);
        }

        public Task WriteToBufferAsync<T>(BufferType bufferType, T message, CancellationToken _)
        {
            WriteToBuffer(bufferType, message);
            return Task.CompletedTask;
        }

        public void AddTransmittingBufferSerializer<TMessage>(Action<IBufferWriter<byte>, TMessage> serializer)
        {
            RegisterDisposable(this.RegisterTransmitMessageCommandHandler<TMessage>(async (message, token) =>
            {
                //serializer.Invoke(transmittingBuffer.Writer, message);
                //await this.SendTransmitBufferDataCommandAsync(transmittingBuffer, token);
            }));
        }

        public void AddReceivingBufferDeserializer<TMessage>(Func<ReadOnlySequence<byte>, (int BytesRead, TMessage Message)> deserializer) { }
            //RegisterDisposable(this.RegisterReceivingBufferUpdatedEventHandler((buffer, token) => DeserializeAsync(buffer, deserializer, token)));

        public void AddReceivingBufferReader(Func<ReadOnlySequence<byte>, int> bufferReader)
        { }// RegisterDisposable(this.RegisterReceivingBufferUpdatedEventHandler(b => ReadBuffer(b, bufferReader)));

        public void AddReceivingBufferReader(Func<ReadOnlySequence<byte>, CancellationToken, Task<int>> bufferReader) { }
            //RegisterDisposable(this.RegisterReceivingBufferUpdatedEventHandler((b, t) => ReadBufferAsync(b, bufferReader, t)));

        private Task DeserializeAsync<TMessage>(IBuffer buffer, Func<ReadOnlySequence<byte>, (int BytesRead, TMessage Message)> deserializer, CancellationToken cancellationToken)
        {
            var reading = buffer.Reader.Read();
            if (reading.Length == 0)
            {
                return Task.CompletedTask;
            }

            var (bytesConsumed, message) = deserializer.Invoke(reading);

            if (bytesConsumed == 0)
            {
                return Task.CompletedTask;
            }

            buffer.Reader.Advance(bytesConsumed);

            return this.SendMessageReceivedEventAsync(message, cancellationToken);
        }
    }

    internal struct BufferActionRequest
    {
        public BufferActionRequest(BufferType bufferType, Action<IBuffer> bufferAction)
        {
            BufferAction = bufferAction;
            BufferType = bufferType;
        }

        public Action<IBuffer> BufferAction { get; }

        public BufferType BufferType { get; }
    }

    internal struct ReadFromBufferCommand
    {
        public ReadFromBufferCommand(BufferType bufferType, Func<ReadOnlySequence<byte>, int> bufferReader)
        {
            BufferType = bufferType;
            BufferReader = bufferReader;
        }

        public BufferType BufferType { get; }

        public Func<ReadOnlySequence<byte>, int> BufferReader { get; }
    }

    internal struct BufferUpdatedEvent
    {
        public BufferUpdatedEvent(BufferType bufferType, IBuffer buffer)
        {
            BufferType = bufferType;
            Buffer = buffer;
        }

        public BufferType BufferType { get; }

        public IBuffer Buffer { get; }
    }
}
