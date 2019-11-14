using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public static class MessageSenderExtensions
    {
        public static void Received<T>(this IMessageSender messageSender, T message) => messageSender.Send(new Received<T>(message));

        public static Task ReceivedAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken) => messageSender.SendAsync(new Received<T>(message), cancellationToken);

        public static void Transmit<T>(this IMessageSender messageSender, T message) => messageSender.Send(new Transmit<T>(message));

        public static Task TransmitAsync<T>(this IMessageSender messageSender, T message, CancellationToken cancellationToken) => messageSender.SendAsync(new Transmit<T>(message), cancellationToken);
    }
}
