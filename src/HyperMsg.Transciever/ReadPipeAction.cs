using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class ReadPipeAction
    {
        private readonly IPipeReader pipeReader;
        private readonly Func<ReadOnlySequence<byte>, int> bufferReader;

        public ReadPipeAction(IPipeReader pipeReader, Func<ReadOnlySequence<byte>, int> bufferReader)
        {
            this.pipeReader = pipeReader ?? throw new ArgumentNullException(nameof(pipeReader));
            this.bufferReader = bufferReader ?? throw new ArgumentNullException(nameof(bufferReader));
        }

        public async Task InvokeAsync(CancellationToken token = default)
        {
            var result = await pipeReader.ReadAsync(token);

            var bytesReaded = bufferReader(result);

            if (bytesReaded == 0)
            {
                return;
            }

            pipeReader.Advance(bytesReaded);
        }
	}
}