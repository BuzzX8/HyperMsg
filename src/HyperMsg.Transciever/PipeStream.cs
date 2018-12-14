using System;
using System.IO.Pipelines;

namespace HyperMsg
{
    public class PipeStream : IStream
    {
        public PipeStream(PipeReader reader, PipeWriter writer)
        {
            Reader = reader ?? throw new ArgumentNullException(nameof(reader));
            Writer = writer ?? throw new ArgumentNullException(nameof(writer));
        }

        public PipeReader Reader { get; }

        public PipeWriter Writer { get; }
    }
}
