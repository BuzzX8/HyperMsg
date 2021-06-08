using System;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class MessagingContextExtensions
    {
        public static Task<TResponse> SendAndWaitMessage<TRequest, TResponse>(this IMessagingContext messagingContext, TRequest request, CancellationToken cancellationToken = default) => 
            messagingContext.SendAndWaitMessage<TRequest, TResponse>(request, _ => true, cancellationToken);

        public static Task<TResponse> SendAndWaitMessage<TRequest, TResponse>(this IMessagingContext messagingContext, TRequest request, Func<TResponse, bool> responsePredicate, CancellationToken cancellationToken = default) => 
            messagingContext.SendAndWaitMessage((sender, token) => sender.SendAsync(request, token), responsePredicate, cancellationToken);

        public static async Task<TResponse> SendAndWaitMessage<TResponse>(this IMessagingContext messagingContext, AsyncAction<IMessageSender> sendAction, Func<TResponse, bool> responsePredicate, CancellationToken cancellationToken = default)
        {
            var waitTask = messagingContext.HandlersRegistry.WaitMessage(responsePredicate, cancellationToken);
            await sendAction.Invoke(messagingContext.Sender, cancellationToken);

            return await waitTask;
        }
    }
}
