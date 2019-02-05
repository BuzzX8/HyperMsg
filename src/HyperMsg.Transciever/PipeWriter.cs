using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Transciever
{
    public class PipeWriter : IPipeWriter
    {
        private readonly IMemoryOwner<byte> memoryOwner;
        private readonly ReadBufferAction readBufferAction;
        private readonly SequencePosition position;

        public PipeWriter(IMemoryOwner<byte> memoryOwner, ReadBufferAction readBufferAction)
        {
            this.memoryOwner = memoryOwner ?? throw new ArgumentNullException(nameof(memoryOwner));
            this.readBufferAction = readBufferAction ?? throw new ArgumentNullException(nameof(readBufferAction));
            position = new SequencePosition(memoryOwner.Memory, 0);
        }

        private Memory<byte> Memory => memoryOwner.Memory;

        public void Advance(int count)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public Task FlushAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
        {
            if (Memory.Length < sizeHint)
            {

            }

            throw new NotImplementedException();
        }

        public Span<byte> GetSpan(int sizeHint = 0)
        {
            throw new NotImplementedException();
        }
    }
}
