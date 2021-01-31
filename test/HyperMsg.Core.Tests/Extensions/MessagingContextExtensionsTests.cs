using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Extensions
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
            var callback = A.Fake<Action>();

            context.SetTimeout(TimeSpan.Zero, callback);            

            A.CallTo(() => callback.Invoke()).MustHaveHappened();
        }

        [Fact]
        public async Task SetTimeout_Does_Not_Invokes_Callback_If_Disposing_Registration()
        {
            var callback = A.Fake<Action>();
            var timeout = TimeSpan.FromSeconds(0.1);
            var registration = context.SetTimeout(timeout, callback);

            registration.Dispose();
            await Task.Delay(timeout);

            A.CallTo(() => callback.Invoke()).MustNotHaveHappened();
        }

        public void Dispose() => host.Dispose();
    }
}
