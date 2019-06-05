using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BackgroundReceiver<T> : BackgroundWorker, IHandler<ReceiveMode>
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

        public void Handle(ReceiveMode message)
        {
            switch (message)
            {
                case ReceiveMode.SetProactive:
                    Stop();
                    break;

                case ReceiveMode.SetReactive:
                    Run();
                    break;
            }
        }

        public Task HandleAsync(ReceiveMode message, CancellationToken token)
        {
            Handle(message);
            return Task.CompletedTask;
        }
    }
}