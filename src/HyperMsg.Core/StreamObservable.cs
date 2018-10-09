using System;
using System.IO;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class StreamObservable : ObservableBase<Memory<byte>>
    {
        private readonly Stream stream;
        private readonly Func<Memory<byte>> bufferProvider;

        private Task observationTask;
        private CancellationTokenSource tokenSource;

        public StreamObservable(Stream stream, Func<Memory<byte>> bufferProvider)
        {
            this.stream = stream ?? throw new ArgumentNullException(nameof(stream));
            this.bufferProvider = bufferProvider ?? throw new ArgumentNullException(nameof(bufferProvider));
            tokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            observationTask = Task.Run(ObserveStream);
        }

        public void Stop()
        {

        }

        private async Task ObserveStream()
        {
            var token = tokenSource.Token;

            while(!token.IsCancellationRequested)
            {
                var buffer = bufferProvider();
                //int readed = await stream.ReadAsync()
            }
        }

        protected override IDisposable SubscribeCore(IObserver<Memory<byte>> observer)
        {
            return null;
        }
    }
}
