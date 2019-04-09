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

        public MessageReceiver(DeserializeFunc<T> deserialize, Memory<byte> buffer, ReadAsyncFunc readAsync)
        {
            this.deserialize = deserialize ?? throw new ArgumentNullException(nameof(deserialize));
            this.buffer = buffer;
            this.readAsync = readAsync ?? throw new ArgumentNullException(nameof(readAsync));
        }

        public T Receive() => ReceiveAsync(CancellationToken.None).GetAwaiter().GetResult();

        public async Task<T> ReceiveAsync(CancellationToken token)
        {
            var readed = await readAsync.Invoke(buffer, token);
            var result = deserialize.Invoke(new ReadOnlySequence<byte>(buffer.Slice(0, readed)));

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
