using System;
using System.Buffers;
using System.IO.Pipelines;

namespace HyperMsg
{
    public class MessageListener<T>
    {
        //private readonly PipeReaderListener readerListener;
        private readonly Func<ReadOnlySequence<byte>, DeserializationResult<T>> deserializer;
        private readonly IObserver<T> observer;

        public MessageListener(PipeReader pipeReader, Func<ReadOnlySequence<byte>, DeserializationResult<T>> deserializer, IObserver<T> observer)
        {
            //readerListener = new PipeReaderListener(pipeReader, ReadBuffer);
            this.deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            this.observer = observer ?? throw new ArgumentNullException(nameof(observer));
        }

	    public IDisposable Run()
        {
            //var disposable = readerListener.Run();
            //Started?.Invoke(this, EventArgs.Empty);
            //return disposable;
            return null;
        }

		private int ReadBuffer(ReadOnlySequence<byte> buffer)
        {
            if (buffer.IsEmpty)
            {
                return 0;
            }

            var result = deserializer(buffer);

            if (result.Message != null)
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
