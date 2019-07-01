using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BackgroundReceiver<T> : BackgroundWorker
    {
        private readonly DeserializeFunc<T> deserialize;
        private readonly IBufferReader bufferReader;
        private readonly AsyncHandler<T> messageHandler;

        public BackgroundReceiver(DeserializeFunc<T> deserialize, IBufferReader bufferReader, AsyncHandler<T> messageHandler)
        {
            this.deserialize = deserialize ?? throw new ArgumentNullException(nameof(deserialize));
            this.bufferReader = bufferReader ?? throw new ArgumentNullException(nameof(bufferReader));
            this.messageHandler = messageHandler ?? throw new ArgumentNullException(nameof(messageHandler));
        }

        protected override async Task DoWorkIterationAsync(CancellationToken cancellationToken)
        {
            var buffer = await bufferReader.ReadAsync(cancellationToken);
            var result = deserialize.Invoke(buffer);

            if (result.MessageSize > 0)
            {
                bufferReader.Advance(result.MessageSize);
                await messageHandler.Invoke(result.Message, cancellationToken);
            }
        }

        public Task HandleTransportEventAsync(TransportEventArgs eventArgs, CancellationToken cancellationToken)
        {
            switch (eventArgs.Event)
            {
                case TransportEvent.Opened:
                    Run();
                    break;

                case TransportEvent.Closed:
                    Stop();
                    break;
            }

            return Task.CompletedTask;
        }
    }
}