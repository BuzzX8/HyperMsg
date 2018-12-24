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

            public PipeReaderProxy(PipeReader reader)
            {
                this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
            }

            public void Advance(int length)
            {
                
            }

            public ReadOnlySequence<byte> Read() => ReadAsync().GetAwaiter().GetResult();

            public async Task<ReadOnlySequence<byte>> ReadAsync(CancellationToken token = default)
            {
                var readResult = await reader.ReadAsync(token);
                return readResult.Buffer;
            }
        }

        private class PipeWriterProxy : IPipeWriter
        {
            private readonly PipeWriter writer;

            public PipeWriterProxy(PipeWriter writer)
            {
                this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            }

            public void Advance(int count) => writer.Advance(count);

            public void Flush() => writer.FlushAsync().GetAwaiter().GetResult();

            public Task FlushAsync(CancellationToken token = default) => writer.FlushAsync(token).AsTask();

            public Memory<byte> GetMemory(int sizeHint = 0) => writer.GetMemory(sizeHint);

            public Span<byte> GetSpan(int sizeHint = 0) => writer.GetSpan(sizeHint);
        }

        public PipeProxy(PipeReader reader, PipeWriter writer)
        {
            Reader = new PipeReaderProxy(reader);
            Writer = new PipeWriterProxy(writer);
        }

        public IPipeReader Reader { get; }

        public IPipeWriter Writer { get; }
    }
}
