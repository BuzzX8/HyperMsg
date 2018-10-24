using System;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class PipeMessageBuffer<T> : IMessageBuffer<T>
    {
        private readonly PipeWriter writer;
        private readonly SerializeAction<T> serializer;
        
        public PipeMessageBuffer(PipeWriter writer, SerializeAction<T> serializer)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

		public Task<FlushResult> FlushAsync(CancellationToken token = default) => writer.FlushAsync(token).AsTask();

		public void Write(T message) => serializer(writer, message);
    }
}
