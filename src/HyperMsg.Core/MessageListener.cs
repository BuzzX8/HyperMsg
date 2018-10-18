using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageListener<T> : IObserver<Memory<byte>> where T : class
    {
        private readonly Pipe pipe;
        private readonly PipeReaderListener readerListener;
        private readonly DeserializeFunc<T> deserializer;
        private readonly IObserver<T> observer;

        public MessageListener(DeserializeFunc<T> deserializer, IObserver<T> observer)
        {
            pipe = new Pipe();
            readerListener = new PipeReaderListener(pipe.Reader, ReadBuffer);
            this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
        }

        public IBufferWriter<byte> GetWriter() => pipe.Writer;

        public void Start()
        {
            readerListener.Start();
            Started?.Invoke(this, EventArgs.Empty);
        }

        public void OnCompleted()
        { }

        public void OnError(Exception error)
        { }

        public void OnNext(Memory<byte> value)
        {
            var writer = pipe.Writer;
            writer.Write(value.Span);
            writer.FlushAsync();
        }

        private int ReadBuffer(ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsEmpty)
            {
                return 0;
            }

            var result = deserializer(buffer);

            if (result.Message != default(T))
            {
                observer.OnNext(result.Message);
            }
            
            DeserializerInvoked?.Invoke(this, EventArgs.Empty);

            return result.BytesConsumed;
        }

        public event EventHandler Started;
        public event EventHandler DeserializerInvoked;
    }
}
