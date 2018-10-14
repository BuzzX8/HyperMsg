using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
	public class StreamListener : ObservableListener<Memory<byte>>
	{
		private readonly Func<Stream> streamProvider;
		private readonly Func<Stream, Task<Memory<byte>>> streamReader;

		public StreamListener(Func<Stream> streamProvider, Func<Stream, Task<Memory<byte>>> streamReader, IObserver<Memory<byte>> observer) : base(observer)
		{
			this.streamProvider = streamProvider ?? throw new ArgumentNullException(nameof(streamProvider));
			this.streamReader = streamReader ?? throw new ArgumentNullException(nameof(streamReader));
		}

		protected override async Task DoListening(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
                var stream = streamProvider();
				var buffer = await streamReader(stream);
                OnNext(buffer);
			}
		}
	}
}