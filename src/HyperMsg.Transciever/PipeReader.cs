using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Transciever
{
    public class PipeReader : IPipeReader
    {
        public void Advance(long offset)
        {
            throw new NotImplementedException();
        }

        public ReadOnlySequence<byte> Read()
        {
            throw new NotImplementedException();
        }

        public Task<ReadOnlySequence<byte>> ReadAsync(CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }
}
