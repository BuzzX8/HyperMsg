using System;
using System.IO.Pipelines;
using System.Linq;
using Xunit;

namespace HyperMsg
{
    public class PipeProxyTests
    {
        [Fact]
        public void Reader_Returns_Data_Written_Into_Writer()
        {
            var pipe = new Pipe();
            var proxy = new PipeProxy(pipe.Reader, pipe.Writer);
            var data = Guid.NewGuid().ToByteArray();
            var count = data.Length / 2;
            var expected = data.Take(count).ToArray();

            var buffer = proxy.Writer.GetSpan(data.Length);
            expected.CopyTo(buffer);
            proxy.Writer.Advance(count);
            proxy.Writer.Flush();

            var result = proxy.Reader.Read().First.ToArray();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Reader_Advances_Reading_Position()
        {
            var pipe = new Pipe();
            var proxy = new PipeProxy(pipe.Reader, pipe.Writer);
            var data = Guid.NewGuid().ToByteArray();
            var count = data.Length / 2;
            var expected = data.Skip(count).ToArray();

            var buffer = proxy.Writer.GetSpan(data.Length);
            expected.CopyTo(buffer);
            proxy.Writer.Advance(data.Length);
            proxy.Writer.Flush();

            proxy.Reader.Read();
            proxy.Reader.Advance(count);
            var actual = proxy.Reader.Read().First.ToArray();

            Assert.Equal(expected, actual);
        }
    }
}
