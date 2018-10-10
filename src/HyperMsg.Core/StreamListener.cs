using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
	public class StreamListener : ListenerBase
	{
		private readonly Stream stream;
		private readonly Func<Stream, Task<Memory<byte>>> streamReader;

		public StreamListener(Stream stream, Func<Stream, Task<Memory<byte>>> streamReader)
		{
			this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
			this.streamReader = streamReader ?? throw new ArgumentNullException(nameof(streamReader));
		}

		protected override async Task DoListening(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				var buffer = await streamReader(stream);
			}
		}
	}
}