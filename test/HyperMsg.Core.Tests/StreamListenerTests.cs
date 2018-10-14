using System;
using System.IO;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
	public class StreamListenerTests
	{
        [Fact]
        public void StreamReaded_Passess_Bytes_Receivrd_From_Reader()
        {
            var bytes = Guid.NewGuid().ToByteArray();
	        var actual = (byte[])null;
			var stream = new MemoryStream();
	        var @event = new ManualResetEventSlim();
			var observer = Observer.Create<Memory<byte>>(b =>
			{
				actual = b.ToArray();
				@event.Set();
			});
	        var listener = new StreamListener(() => stream, s => ReadStreamAsync(s, bytes), observer);
	        listener.Next += (s, e) => listener.Stop();
			stream.Write(bytes);
            stream.Seek(0, SeekOrigin.Begin);

            listener.Start();
            @event.Wait(TimeSpan.FromSeconds(1));

            Assert.Equal(bytes, actual);
        }

		private async Task<Memory<byte>> ReadStreamAsync(Stream stream, byte[] bytes)
		{
			var buffer = new Memory<byte>(new byte[bytes.Length], 0, bytes.Length);
			int readed = await stream.ReadAsync(buffer);
			return buffer.Slice(0, readed);
		}
	}
}