using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class StreamFilter : Stream, IStreamFilter
    {
        private readonly IBufferReader receiveBufferReader;
        private readonly IBuffer transmitBuffer;

        internal StreamFilter(IBufferReader receiveBufferReader, IBuffer transmitBuffer)
        {
            this.receiveBufferReader = receiveBufferReader ?? throw new ArgumentNullException(nameof(receiveBufferReader));
            this.transmitBuffer = transmitBuffer ?? throw new ArgumentNullException(nameof(transmitBuffer));
        }

        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override bool CanSeek => false;

        public override long Length => throw new NotSupportedException();

        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public Stream Stream => this;

        public override int Read(Span<byte> buffer)
        {
            var readed = receiveBufferReader.Read();
            throw new System.NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count) => Read(new Span<byte>(buffer, offset, count));

        public override void Write(ReadOnlySpan<byte> buffer)
        {
            var buff = transmitBuffer.Writer.GetSpan();
            buffer.CopyTo(buff);
            transmitBuffer.Writer.Advance(buffer.Length);
        }

        public override void Write(byte[] buffer, int offset, int count) => Write(new ReadOnlySpan<byte>(buffer, offset, count));

        public override void Flush() => transmitBuffer.Flush();

        public override Task FlushAsync(CancellationToken cancellationToken) => transmitBuffer.FlushAsync(cancellationToken);

        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();

        public override void SetLength(long value) => throw new NotSupportedException();
    }

    public interface IStreamFilter
    {
        Stream Stream { get; }
    }

    public interface IStreamContext
    {
        Stream Left { get; }

        Stream Right { get; }
    }

    internal class StreamContext : IStreamContext
    {
        public StreamContext(IBufferContext bufferContext)
        {
            Left = new StreamFilter(bufferContext.ReceivingBuffer.Reader, bufferContext.TransmittingBuffer);
            Right = new StreamFilter(bufferContext.TransmittingBuffer.Reader, bufferContext.ReceivingBuffer);
        }

        public Stream Left { get; }

        public Stream Right { get; }
    }
}