using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class CompositeHandler : IHandler
    {
        private readonly IHandlerRepository handlerRepository;

        public CompositeHandler(IHandlerRepository handlerRepository)
        {
            this.handlerRepository = handlerRepository ?? throw new ArgumentNullException(nameof(handlerRepository));
        }

        public void Handle<T>(T message)
        {
            var handlers = handlerRepository.GetHandlers<T>() ?? throw new InvalidOperationException();

            foreach (var handler in handlers)
            {
                handler.Handle(message);
            }
        }

        public async Task HandleAsync<T>(T message, CancellationToken cancellationToken = default)
        {
            var handlers = handlerRepository.GetHandlers<T>() ?? throw new InvalidOperationException();

            foreach (var handler in handlers)
            {
                await handler.HandleAsync(message, cancellationToken);
            }
        }
    }
}
