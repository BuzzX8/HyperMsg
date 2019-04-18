using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using ReadAsyncFunc = System.Func<System.Memory<byte>, System.Threading.CancellationToken, System.Threading.Tasks.Task<int>>;

namespace HyperMsg
{
    public class BufferReader
    {
        private readonly Memory<byte> buffer;
        private readonly ReadAsyncFunc readAsync;

        private int position;

        public BufferReader(Memory<byte> buffer, ReadAsyncFunc readAsync)
        {
            this.buffer = buffer;
            this.readAsync = readAsync ?? throw new ArgumentNullException(nameof(readAsync));
            position = 0;
        }

        public void Advance(int count)
        {
            if (count < position)
            {
                buffer.Slice(count).CopyTo(buffer);
            }

            position -= count;
        }

        public ReadOnlySequence<byte> Read() => ReadAsync(CancellationToken.None).GetAwaiter().GetResult();

        public async Task<ReadOnlySequence<byte>> ReadAsync(CancellationToken token)
        {
            var readed = await readAsync.Invoke(buffer.Slice(position), token);
            position += readed;

            return new ReadOnlySequence<byte>(buffer.Slice(0, position));
        }
    }
}
