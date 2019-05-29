using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class BackgroundReceiver<T> : BackgroundWorker, IHandler<ReceiveMode>
    {
        private readonly IReceiver<T> messageReceiver;
        private readonly Func<IEnumerable<IHandler<T>>> handlersProvider;

        public BackgroundReceiver(IReceiver<T> messageReceiver, Func<IEnumerable<IHandler<T>>> handlersProvider)
        {
            this.messageReceiver = messageReceiver ?? throw new ArgumentNullException(nameof(messageReceiver));
            this.handlersProvider = handlersProvider ?? throw new ArgumentNullException(nameof(handlersProvider));
        }

        protected override async Task DoWorkIterationAsync(CancellationToken cancellationToken)
        {
            var message = await messageReceiver.ReceiveAsync(cancellationToken);
            var handlers = handlersProvider.Invoke();

            if (handlers == null)
            {
                return;
            }

            foreach (var handler in handlers)
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