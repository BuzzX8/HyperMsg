using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageListener<T> where T : class
    {
        private readonly PipeReaderListener readerListener;
        private readonly DeserializeFunc<T> deserializer;
        private readonly IObserver<T> observer;

        public MessageListener(PipeReader pipeReader, DeserializeFunc<T> deserializer, IObserver<T> observer)
        {
            readerListener = new PipeReaderListener(pipeReader, ReadBuffer);
            this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
        }

	    public IDisposable Run()
        {
            var disposable = readerListener.Run();
            Started?.Invoke(this, EventArgs.Empty);
            return disposable;
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
