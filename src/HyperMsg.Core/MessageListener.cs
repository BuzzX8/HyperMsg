using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageListener<T> : IMessageBuffer<ReadOnlyMemory<byte>> where T : class
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

	    public IBufferWriter<byte> Writer => pipe.Writer;

	    public IDisposable Run()
        {
            var disposable = readerListener.Run();
            Started?.Invoke(this, EventArgs.Empty);
            return disposable;
        }

	    public void Write(ReadOnlyMemory<byte> message)
	    {
		    Writer.Write(message.Span);
	    }

	    public Task<FlushResult> FlushAsync(CancellationToken token = default)
	    {
		    return pipe.Writer.FlushAsync(token).AsTask();
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
