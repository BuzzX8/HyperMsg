using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BackgroundReceiver<T> : BackgroundWorker, IHandler<ReceiveMode>
    {
        private readonly IReceiver<T> messageReceiver;
        private readonly ISender sender;

        public BackgroundReceiver(IReceiver<T> messageReceiver, ISender sender)
        {
            this.messageReceiver = messageReceiver ?? throw new ArgumentNullException(nameof(messageReceiver));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        protected override async Task DoWorkIterationAsync(CancellationToken cancellationToken)
        {
            var message = await messageReceiver.ReceiveAsync(cancellationToken);
            await sender.SendAsync(message, cancellationToken);
        }

        public void Handle(ReceiveMode message)
        {
            switch (message)
            {
                case ReceiveMode.Proactive:
                    Stop();
                    break;

                case ReceiveMode.Reactive:
                    Run();
                    break;
            }
        }

        public Task HandleAsync(ReceiveMode message, CancellationToken token = default)
        {
            Handle(message);
            return Task.CompletedTask;
        }
    }
}