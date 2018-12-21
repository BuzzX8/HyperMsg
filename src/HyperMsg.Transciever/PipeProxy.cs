using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class PipeProxy : IPipe
    {
        private class PipeReaderProxy : IPipeReader
        {
            private readonly PipeReader reader;

            public void Advance(int length)
            {
                throw new NotImplementedException();
            }

            public ReadOnlySequence<byte> Read()
            {
                throw new NotImplementedException();
            }

            public Task<ReadOnlySequence<byte>> ReadAsync(CancellationToken token = default)
            {
                throw new NotImplementedException();
            }
        }

        private class PipeWriterProxy : IPipeWriter
        {
            public void Flush()
            {
                throw new NotImplementedException();
            }

            public Task FlushAsync(CancellationToken token = default)
            {
                throw new NotImplementedException();
            }

            public Memory<byte> GetMemory(int sizeHint)
            {
                throw new NotImplementedException();
            }
        }

        public PipeProxy(PipeReader reader, PipeWriter writer)
        {
            //Reader = reader ?? throw new ArgumentNullException(nameof(reader));
            //Writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public IPipeReader Reader { get; }

        public IPipeWriter Writer { get; }
    }
}
