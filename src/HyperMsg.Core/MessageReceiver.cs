using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessageReceiver<T> : IReceiver<T>
    {
        private readonly DeserializeFunc<T> deserialize;
        private readonly IBufferReader bufferReader;        

        public MessageReceiver(DeserializeFunc<T> deserialize, IBufferReader bufferReader)
        {
            this.deserialize = deserialize ?? throw new ArgumentNullException(nameof(deserialize));
            this.bufferReader = bufferReader ?? throw new ArgumentNullException(nameof(bufferReader));
        }

        public T Receive() => ReceiveAsync(CancellationToken.None).GetAwaiter().GetResult();

        public async Task<T> ReceiveAsync(CancellationToken token)
        {
            var readed = await bufferReader.ReadAsync(token);
            var result = deserialize.Invoke(readed);

            if (result.BytesConsumed > 0)
            {
                bufferReader.Advance(result.BytesConsumed);
                return result.Message;
            }

            const int DefaultReadCount = 50;
            int readCount = 0;

            while (result.BytesConsumed == 0)
            {
                readed = await bufferReader.ReadAsync(token);
                result = deserialize.Invoke(readed);
                readCount++;

                if (readCount == DefaultReadCount)
                {
                    throw new InvalidOperationException();
                }
            }

            bufferReader.Advance(result.BytesConsumed);

            return result.Message;
        }
    }
}
