using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class TimerService : MessagingObject, IHostedService
    {
        private readonly Dictionary<Guid, Timer> timers = new();

        public TimerService(IMessagingContext messagingContext) : base(messagingContext)
        {
            RegisterHandler<TimerMessages.SetTimeout>(SetTimeout);
        }

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        private void SetTimeout(TimerMessages.SetTimeout timeout)
        {
            var timer = new Timer(TimerCallback, timeout.Id, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            timers.Add(timeout.Id, timer);
            timer.Change(timeout.Timeout, Timeout.InfiniteTimeSpan);
        }

        private void TimerCallback(object state)
        {
            var id = (Guid)state;
            timers.Remove(id);
            Send(new TimerMessages.Timeout(id));
        }
    }

    internal class TimerMessages
    {
        internal class SetTimeout
        {
            internal SetTimeout(Guid id, TimeSpan timeout) => (Id, Timeout) = (id, timeout);

            internal Guid Id { get; }

            internal TimeSpan Timeout { get; }
        }

        internal class Timeout
        {
            internal Timeout(Guid id) => Id = id;

            internal Guid Id { get; }
        }
    }
}
