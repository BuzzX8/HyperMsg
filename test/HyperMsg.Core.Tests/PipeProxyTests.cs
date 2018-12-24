using System;
using System.IO.Pipelines;
using Xunit;

namespace HyperMsg
{
    public class PipeProxyTests
    {
        [Fact]
        public void Reads_All_Written_Data()
        {
            var pipe = new Pipe();
            var proxy = new PipeProxy(pipe.Reader, pipe.Writer);
            var data = Guid.NewGuid().ToByteArray();

            var buffer = proxy.Writer.GetSpan(data.Length);
            data.CopyTo(buffer);
            proxy.Writer.Advance(data.Length);
            proxy.Writer.Flush();

            var result = proxy.Reader.Read().First.ToArray();
            Assert.Equal(data, result);
        }
    }
}
