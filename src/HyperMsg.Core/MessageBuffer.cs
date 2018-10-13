using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageBuffer<T> : IMessageBuffer<T>
    {
        private readonly PipeWriter writer;
        private readonly Action<IBufferWriter<byte>, T> serializer;
        
        public MessageBuffer(PipeWriter writer, Action<IBufferWriter<byte>, T> serializer)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public Task<FlushResult> FlushAsync()
        {
            return writer.FlushAsync().AsTask();
        }

        public void Write(T message)
        {
            serializer(writer, message);
        }
    }
}
