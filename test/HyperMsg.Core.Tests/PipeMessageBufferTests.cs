using System;
using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class PipeMessageBufferTests
    {
        [Fact]
        public async Task Write_Serializes_Message_Into_Writer()
        {
            var pipe = new Pipe();
            var messageBuffer = new PipeMessageBuffer<string>(pipe.Writer, WriteString);
            var expected = Guid.NewGuid().ToString();
            messageBuffer.Write(expected);

            await messageBuffer.FlushAsync();
            var result = await pipe.Reader.ReadAsync();
            var buffer = result.Buffer.First;

            var actual = Encoding.UTF8.GetString(buffer.Span);

            Assert.Equal(expected, actual);
        }

        private void WriteString(IBufferWriter<byte> writer, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            writer.Write(bytes);
        }
    }
}
