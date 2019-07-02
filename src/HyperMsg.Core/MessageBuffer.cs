using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageBuffer<T> : IMessageBuffer<T>, IMessageSender<T>
    {
        private readonly IBufferWriter<byte> bufferWriter;
        private readonly SerializeAction<T> serializeAction;
        private readonly AsyncAction flushDelegate;

        public MessageBuffer(IBufferWriter<byte> bufferWriter, SerializeAction<T> serializeAction, AsyncAction flushDelegate)
        {
            this.bufferWriter = bufferWriter ?? throw new ArgumentNullException(nameof(bufferWriter));
            this.serializeAction = serializeAction ?? throw new ArgumentNullException(nameof(serializeAction));
            this.flushDelegate = flushDelegate ?? throw new ArgumentNullException(nameof(flushDelegate));
        }

        public Task FlushAsync(CancellationToken cancellationToken) => flushDelegate.Invoke(cancellationToken);

        public async Task SendAsync(T message, CancellationToken cancellationToken)
        {
            Write(message);
            await FlushAsync(cancellationToken);
        }

        public void Write(T message) => serializeAction.Invoke(bufferWriter, message);
    }
}
