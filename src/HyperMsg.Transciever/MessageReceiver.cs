using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using ReadAsyncFunc = System.Func<System.Memory<byte>, System.Threading.CancellationToken, System.Threading.Tasks.Task<int>>;

namespace HyperMsg.Transciever
{
    public class MessageReceiver<T> : IReceiver<T>
    {
        private readonly DeserializeFunc<T> deserialize;
        private readonly Memory<byte> buffer;
        private readonly ReadAsyncFunc readAsync;

        private int position;

        public MessageReceiver(DeserializeFunc<T> deserialize, Memory<byte> buffer, ReadAsyncFunc readAsync)
        {
            this.deserialize = deserialize ?? throw new ArgumentNullException(nameof(deserialize));
            this.buffer = buffer;
            this.readAsync = readAsync ?? throw new ArgumentNullException(nameof(readAsync));
            position = 0;
        }

        public T Receive() => ReceiveAsync(CancellationToken.None).GetAwaiter().GetResult();

        public async Task<T> ReceiveAsync(CancellationToken token)
        {
            var readed = await readAsync.Invoke(buffer.Slice(position), token);
            position += readed;
            var result = deserialize.Invoke(new ReadOnlySequence<byte>(buffer.Slice(0, position)));

            if (result.BytesConsumed < position)
            {
                buffer.Slice(result.BytesConsumed).CopyTo(buffer);                
            }

            position -= result.BytesConsumed;

            if (result.BytesConsumed > 0)
            {
                return result.Message;
            }

            while (result.BytesConsumed == 0)
            {
                result = deserialize.Invoke(new ReadOnlySequence<byte>(buffer.Slice(0, readed)));
            }

            return result.Message;
        }
    }
}
