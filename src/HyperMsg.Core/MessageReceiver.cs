using System;
using System.Threading;
using System.Threading.Tasks;
using ReadAsyncFunc = System.Func<System.Memory<byte>, System.Threading.CancellationToken, System.Threading.Tasks.Task<int>>;

namespace HyperMsg
{
    public class MessageReceiver<T> : IReceiver<T>
    {
        private readonly DeserializeFunc<T> deserialize;        
        private readonly ReadAsyncFunc readAsync;
        private readonly BufferReader bufferReader;        

        public MessageReceiver(DeserializeFunc<T> deserialize, Memory<byte> buffer, ReadAsyncFunc readAsync)
        {
            this.deserialize = deserialize ?? throw new ArgumentNullException(nameof(deserialize));            
            this.readAsync = readAsync ?? throw new ArgumentNullException(nameof(readAsync));            
            bufferReader = new BufferReader(buffer, readAsync);
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

            while (result.BytesConsumed == 0)
            {
                readed = await bufferReader.ReadAsync(token);
                result = deserialize.Invoke(readed);                
            }

            bufferReader.Advance(result.BytesConsumed);

            return result.Message;
        }
    }
}
