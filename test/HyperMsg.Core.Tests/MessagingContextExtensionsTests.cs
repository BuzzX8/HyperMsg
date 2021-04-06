using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using Xunit;

namespace HyperMsg
{
    public class MessagingContextExtensionsTests : IDisposable
    {
        private readonly ServiceHost host;
        private readonly IMessagingContext context;

        public MessagingContextExtensionsTests()
        {
            host = ServiceHost.CreateDefault(services =>
            {
                services.AddTimerService();
            });
            context = host.GetRequiredService<IMessagingContext>();
            host.StartAsync().Wait();
        }

        [Fact]
        public void SetTimeout_Invokes_Callback()
        {
            var @event = new ManualResetEventSlim();

            context.SetTimeout(TimeSpan.Zero, () => @event.Set());
            @event.Wait(TimeSpan.FromSeconds(1));

            Assert.True(@event.IsSet);
        }

        [Fact]
        public void SetTimeout_Does_Not_Invokes_Callback_If_Disposing_Registration()
        {
            var @event = new ManualResetEventSlim();
            var timeout = TimeSpan.FromSeconds(0.1);
            var registration = context.SetTimeout(timeout, () => @event.Set());

            registration.Dispose();
            @event.Wait(timeout * 2);

            Assert.False(@event.IsSet);
        }

        [Fact]
        public void SetInterval_Periodically_Invokes_Handler()
        {
            var @event = new ManualResetEventSlim();
            var timeout = TimeSpan.FromSeconds(0.1);
            var expected = 4;
            var actual = 0;

            var registration = context.SetInterval(timeout, () =>
            {
                actual++;
                if (actual == expected)
                {
                    @event.Set();
                }
            });
            @event.Wait(timeout * 10);
            registration.Dispose();

            Assert.True(@event.IsSet);
            Assert.Equal(expected, actual);
        }

        public void Dispose() => host.Dispose();
    }
}
