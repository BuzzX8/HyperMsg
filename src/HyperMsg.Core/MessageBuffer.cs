using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageBuffer<T> : IMessageBuffer<T>, ISender<T>
    {        
        private readonly SerializeAction<T> serializeAction;
        private readonly ByteBufferWriter writer;
        private readonly Func<Memory<byte>, CancellationToken, Task> writeAsync;

        public MessageBuffer(SerializeAction<T> serializeAction, Memory<byte> buffer, Func<Memory<byte>, CancellationToken, Task> writeAsync)
        {            
            this.serializeAction = serializeAction ?? throw new ArgumentNullException(nameof(serializeAction));
            this.writeAsync = writeAsync ?? throw new ArgumentNullException(nameof(writeAsync));
            writer = new ByteBufferWriter(buffer);
        }

        public void Flush() => FlushAsync().GetAwaiter().GetResult();

        public async Task FlushAsync(CancellationToken token = default)
        {
            await writeAsync.Invoke(writer.CommitedMemory, token);
            writer.Reset();
        }

        public void Send(T message)
        {
            Write(message);
            Flush();
        }

        public async Task SendAsync(T message, CancellationToken token = default)
        {
            Write(message);
            await FlushAsync(token);
        }

        public void Write(T message) => serializeAction.Invoke(writer, message);
    }
}
