using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class MessagingService : MessagingObject, IHostedService
    {
        public MessagingService(IMessagingContext messagingContext) : base(messagingContext)
        {
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            RegisterDefaultDisposables();
            return Task.CompletedTask;
        }

        public virtual Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
