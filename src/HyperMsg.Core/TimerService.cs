using HyperMsg.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace HyperMsg
{
    public class TimerService : MessagingService
    {
        private readonly ConcurrentDictionary<Guid, Timer> timers = new();

        public TimerService(IMessagingContext messagingContext) : base(messagingContext)
        { }

        protected override IEnumerable<IDisposable> GetAutoDisposables()
        {
            yield return RegisterHandler<TimerMessages.SetInterval>(SetInterval);
            yield return RegisterHandler<TimerMessages.SetTimeout>(SetTimeout);
            yield return RegisterHandler<TimerMessages.DisposeTimer>(DisposeTimer);
        }

        private void DisposeTimer(TimerMessages.DisposeTimer message)
        {
            var id = message.Id;
            
            if (!timers.ContainsKey(id))
            {
                return;
            }

            if (timers.TryRemove(id, out var timer))
            {
                timer.Dispose();
            }
        }

        private void SetTimeout(TimerMessages.SetTimeout timeout)
        {
            var timer = new Timer(TimeoutCallback, timeout.Id, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            if (timers.TryAdd(timeout.Id, timer))
            {
                timer.Change(timeout.Timeout, Timeout.InfiniteTimeSpan);
            }
        }

        private void TimeoutCallback(object state)
        {
            var id = (Guid)state;
            if (timers.TryRemove(id, out var timer))
            {
                timer.Dispose();
            }
            Send(new TimerMessages.Timeout(id));
        }

        private void SetInterval(TimerMessages.SetInterval interval)
        {
            var timer = new Timer(IntervalCallback, interval.Id, Timeout.InfiniteTimeSpan, Timeout.InfiniteTimeSpan);
            if (timers.TryAdd(interval.Id, timer))
            {
                timer.Change(TimeSpan.Zero, interval.Interval);
            }
        }

        private void IntervalCallback(object state)
        {
            var id = (Guid)state;
            Send(new TimerMessages.Interval(id));
        }

        public override void Dispose()
        {
            base.Dispose();

            foreach(var timer in timers.Values)
            {
                timer.Dispose();
            }
        }
    }

    

    internal class TimerRegistration : IDisposable
    {
        private readonly Guid id;
        private readonly IDisposable handlerRegistration;
        private readonly IMessageSender messageSender;

        internal TimerRegistration(Guid id, IDisposable handlerRegistration, IMessageSender messageSender)
        {
            this.id = id;
            this.handlerRegistration = handlerRegistration;
            this.messageSender = messageSender;
        }

        public void Dispose()
        {
            handlerRegistration.Dispose();
            messageSender.Send(new TimerMessages.DisposeTimer(id));
        }
    }
}
