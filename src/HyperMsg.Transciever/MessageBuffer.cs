using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Transciever
{
    class MessageBuffer<T> : IMessageBuffer<T>
    {
        private readonly IPipeWriter writer;
        private readonly Action<IBufferWriter<byte>, T> serializeAction;

        public MessageBuffer(IPipeWriter writer, Action<IBufferWriter<byte>, T> serializeAction)
        {
            this.writer = writer ?? throw new ArgumentNullException(nameof(writer));
            this.serializeAction = serializeAction ?? throw new ArgumentNullException(nameof(serializeAction));
        }

        public Task FlushAsync(CancellationToken token = default) => writer.FlushAsync(token);

        public void Write(T message) => serializeAction.Invoke(writer, message);
    }
}
