using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class PipeMessageBuffer<T> : IMessageBuffer<T>
    {
        private readonly PipeWriter writer;
        private readonly Action<IBufferWriter<byte>, T> serializer;
        
        public PipeMessageBuffer(PipeWriter writer, Action<IBufferWriter<byte>, T> serializer)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

		public Task FlushAsync(CancellationToken token = default) => writer.FlushAsync(token).AsTask();

		public void Write(T message) => serializer(writer, message);
    }
}
