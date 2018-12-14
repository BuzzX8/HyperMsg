using System.IO.Pipelines;

namespace HyperMsg
{
    public interface IStream
    {
        PipeReader Reader { get; }

        PipeWriter Writer { get; }
    }
}
