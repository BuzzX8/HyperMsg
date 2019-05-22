using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BackgroundReceiver<T> : BackgroundWorker, IHandler<ReceiveMode>
    {
        private readonly IReceiver<T> messageReceiver;
        private readonly IEnumerable<IHandler<T>> messageHandlers;

        public BackgroundReceiver(IReceiver<T> messageReceiver, IEnumerable<IHandler<T>> messageHandlers)
        {
            this.messageReceiver = messageReceiver ?? throw new ArgumentNullException(nameof(messageReceiver));
            this.messageHandlers = messageHandlers ?? throw new ArgumentNullException(nameof(messageHandlers));
        }

        protected override async Task DoWorkIterationAsync(CancellationToken cancellationToken)
        {
            var message = await messageReceiver.ReceiveAsync(cancellationToken);

            foreach (var handler in messageHandlers)
            {
                await handler.HandleAsync(message, cancellationToken);
            }
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