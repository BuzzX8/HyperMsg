using System;
using System.IO;
using System.Threading;
using Xunit;

namespace HyperMsg
{
	public class StreamListenerTests
	{
        [Fact]
        public void StreamReaded_Passess_Bytes_Receivrd_From_Reader()
        {
            var bytes = Guid.NewGuid().ToByteArray();
            var stream = new MemoryStream();
            var listener = new StreamListener(stream, async s => 
            {
                var buffer = new Memory<byte>(new byte[bytes.Length], 0, bytes.Length);
                int readed = await stream.ReadAsync(buffer);
                return buffer.Slice(0, readed);
            });
            var @event = new ManualResetEventSlim();
            stream.Write(bytes);
            stream.Seek(0, SeekOrigin.Begin);
            var actual = (byte[])null;
            listener.StreamReaded += b =>
            {
                actual = b.ToArray();
                listener.Stop();
                @event.Set();
            };
            listener.Start();
            @event.Wait(TimeSpan.FromSeconds(1));

            Assert.Equal(bytes, actual);
        }
	}
}