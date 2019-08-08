using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using ReadAsyncFunc = System.Func<System.Memory<byte>, System.Threading.CancellationToken, System.Threading.Tasks.Task<int>>;

namespace HyperMsg
{
    public class BufferReader : IBufferReader<byte>, IDisposable
    {
        private readonly IMemoryOwner<byte> memoryOwner;
        private readonly ReadAsyncFunc readAsync;

        private int position;

        public BufferReader(IMemoryOwner<byte> memoryOwner, ReadAsyncFunc readAsync)
        {
            this.memoryOwner = memoryOwner ?? throw new ArgumentNullException(nameof(memoryOwner));
            this.readAsync = readAsync ?? throw new ArgumentNullException(nameof(readAsync));
            position = 0;
        }

        private Memory<byte> Memory => memoryOwner.Memory;

        public void Advance(int count)
        {
            if (count > position)
            {
                throw new IndexOutOfRangeException();
            }

            if (count < position)
            {
                Memory.Slice(count).CopyTo(Memory);
            }

            position -= count;
        }

        public ReadOnlySequence<byte> Read() => ReadAsync(CancellationToken.None).GetAwaiter().GetResult();

        public async Task<ReadOnlySequence<byte>> ReadAsync(CancellationToken token)
        {
            var readed = await readAsync.Invoke(Memory.Slice(position), token);
            position += readed;

            return new ReadOnlySequence<byte>(Memory.Slice(0, position));
        }

        public void Dispose() => memoryOwner.Dispose();
    }
}
