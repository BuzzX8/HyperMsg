using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg.Extensions
{
    public class MessagingContextExtensionsTests
    {
        [Fact]
        public async Task SetTimeout_()
        {
            var host = ServiceHost.CreateDefault(services =>
            {
                services.AddTimerService();
            });
            host.StartAsync().Wait();
            var context = host.GetRequiredService<IMessagingContext>();
            var callback = A.Fake<Action>();

            context.SetTimeout(TimeSpan.Zero, callback);            

            A.CallTo(() => callback.Invoke()).MustHaveHappened();
        }
    }
}
