using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class MessagingContextExtensions
    {
        public static Task<TResponse> SendAndWaitMessage<TRequest, TResponse>(this IMessagingContext messagingContext, TRequest request, CancellationToken cancellationToken = default) => 
            messagingContext.SendAndWaitMessage<TRequest, TResponse>(request, _ => true, cancellationToken);

        public static async Task<TResponse> SendAndWaitMessage<TRequest, TResponse>(this IMessagingContext messagingContext, TRequest request, Func<TResponse, bool> responsePredicate, CancellationToken cancellationToken = default)
        {
            var waitTask = messagingContext.HandlersRegistry.WaitMessage(responsePredicate, cancellationToken);
            await messagingContext.Sender.SendAsync(request, cancellationToken);

            return await waitTask;
        }
    }
}
