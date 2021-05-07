using System;

namespace HyperMsg.Messages
{
    internal class TimerMessages
    {
        internal struct DisposeTimer
        {
            internal DisposeTimer(Guid id) => Id = id;

            internal Guid Id { get; }
        }

        internal struct SetTimeout
        {
            internal SetTimeout(Guid id, TimeSpan timeout) => (Id, Timeout) = (id, timeout);

            internal Guid Id { get; }

            internal TimeSpan Timeout { get; }
        }

        internal struct Timeout
        {
            internal Timeout(Guid id) => Id = id;

            internal Guid Id { get; }
        }

        internal struct SetInterval
        {
            internal SetInterval(Guid id, TimeSpan interval) => (Id, Interval) = (id, interval);

            internal Guid Id { get; }

            internal TimeSpan Interval { get; }
        }

        internal struct Interval
        {
            internal Interval(Guid id) => Id = id;

            internal Guid Id { get; }
        }
    }
}
