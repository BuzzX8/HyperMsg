using System;

namespace HyperMsg.Extensions
{
    public static class MessagingContextExtensions
    {
        public static IDisposable SetTimeout(this IMessagingContext messagingContext, TimeSpan timeout, Action callback)
        {
            var id = Guid.NewGuid();
            var registration = messagingContext.HandlersRegistry.RegisterHandler<TimerMessages.Timeout>(m =>
            {
                if (m.Id != id)
                {
                    return;
                }
                callback();
            });
            messagingContext.Sender.Send(new TimerMessages.SetTimeout(id, timeout));
            return registration;
        }

        public static IDisposable SetInterval(this IMessagingContext messagingContext, TimeSpan interval, Action callback)
        {
            throw new NotImplementedException();
        }
    }
}
