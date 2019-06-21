using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BackgroundReceiver<T> : BackgroundWorker, IHandler<TransportMessage>
    {
        private readonly DeserializeFunc<T> deserialize;
        private readonly IBufferReader bufferReader;
        private readonly IReceiver<T> messageReceiver;
        private readonly IHandler<T> messageHandler;

        public BackgroundReceiver(DeserializeFunc<T> deserialize, IBufferReader bufferReader, IHandler<T> messageHandler)
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
                await messageHandler.HandleAsync(result.Message, cancellationToken);
            }            
        }

        public void Handle(TransportMessage message)
        {
            switch (message)
            {
                case TransportMessage.Opened:
                    Run();
                    break;

                case TransportMessage.Closed:
                    Stop();
                    break;
            }
        }

        public Task HandleAsync(TransportMessage message, CancellationToken token)
        {
            Handle(message);
            return Task.CompletedTask;
        }
    }
}