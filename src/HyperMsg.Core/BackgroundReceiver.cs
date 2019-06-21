using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BackgroundReceiver<T> : BackgroundWorker, IHandler<TransportMessage>
    {
        private readonly IReceiver<T> messageReceiver;
        private readonly IPublisher publisher;

        public BackgroundReceiver(IReceiver<T> messageReceiver, IPublisher publisher)
        {
            this.messageReceiver = messageReceiver ?? throw new ArgumentNullException(nameof(messageReceiver));
            this.publisher = publisher ?? throw new ArgumentNullException(nameof(publisher));
        }

        protected override async Task DoWorkIterationAsync(CancellationToken cancellationToken)
        {
            var message = await messageReceiver.ReceiveAsync(cancellationToken);
            await publisher.PublishAsync(message, cancellationToken);
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