using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Transciever
{
    public class MessageTransceiver<T> : ITransceiver<T, T>, IObserver<T>
    {
        private readonly ITransceiver<ReadOnlySequence<byte>, ReadOnlySequence<byte>> transport;
        private readonly ISerializer<T> serializer;

        private readonly PipeMessageBuffer<T> messageBuffer;
        private readonly MessageListener<T> messageListener;

        private readonly PipeReaderListener readerListener;

        private readonly Pipe outputPipe;
        private readonly Pipe inputPipe;

        public MessageTransceiver(ISerializer<T> serializer, ITransceiver<ReadOnlySequence<byte>, ReadOnlySequence<byte>> transport)
        {
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            this.transport = transport ?? throw new ArgumentNullException(nameof(transport));

            outputPipe = new Pipe();
            inputPipe = new Pipe();

            messageBuffer = new PipeMessageBuffer<T>(outputPipe.Writer, serializer.Serialize);
            messageListener = new MessageListener<T>(inputPipe.Reader, serializer.Deserialize, this);
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(T value)
        {
            throw new NotImplementedException();
        }

        public void OnNext(ReadOnlySequence<byte> value)
        {
            throw new NotImplementedException();
        }

        public IDisposable Run()
        {
            throw new NotImplementedException();
        }

        public void Send(T message)
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync(T message, CancellationToken token = default)
        {
            messageBuffer.Write(message);
            await messageBuffer.FlushAsync(token);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            throw new NotImplementedException();
        }
    }
}
