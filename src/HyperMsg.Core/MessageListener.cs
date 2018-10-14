using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageListener<T> : IObserver<Memory<byte>>
    {
        private readonly Pipe pipe;
        private readonly StreamListener streamListener;
        private readonly PipeReaderListener readerListener;

        public MessageListener(DeserializeAction<T> deserializer, Func<Stream> streamProvider, IObserver<T> observer)
        {
            pipe = new Pipe();
            streamListener = new StreamListener(streamProvider, ReadStream, this);
            readerListener = new PipeReaderListener(pipe.Reader, ReadBuffer);
        }

        public void Start()
        {
            streamListener.Start();
            readerListener.Start();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnNext(Memory<byte> value)
        {
            throw new NotImplementedException();
        }

        private async Task<Memory<byte>> ReadStream(Stream stream)
        {
            return null;
        }

        private int ReadBuffer(ReadOnlySequence<byte> buffer)
        {
            return -1;
        }
    }
}
