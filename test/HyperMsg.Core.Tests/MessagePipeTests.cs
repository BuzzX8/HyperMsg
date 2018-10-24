using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading;
using Xunit;

namespace HyperMsg
{
    public class MessagePipeTests
    {
        private readonly MemoryStream stream;
        private readonly ManualResetEventSlim @event;
        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        public MessagePipeTests()
        {
            stream = new MemoryStream();
            @event = new ManualResetEventSlim();
        }

        [Fact]
        public void FlushAsync_Sends_Written_Message()
        {
            var expected = Guid.NewGuid().ToString();            
            var buffer = new MessagePipe<string>(WriteString, b =>
            {
                try
                {
                    stream.Write(b.First.Span);
                    return b.First.Length;
                }
                finally
                {
                    @event.Set();
                }
            });
            buffer.Run();

            buffer.Write(expected);
            buffer.FlushAsync().Wait();
            @event.Wait(waitTimeout);
            var actual = Encoding.UTF8.GetString(stream.ToArray());

            Assert.Equal(expected, actual);
        }

        private void WriteString(IBufferWriter<byte> writer, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            writer.Write(bytes);
        }


    }
}
