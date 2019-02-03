using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg.Transciever
{
    public class Pipe : IPipe
    {
        public Pipe(IPipeReader reader, IPipeWriter writer)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public IPipeReader Reader { get; }

        public IPipeWriter Writer { get; }
    }
}
