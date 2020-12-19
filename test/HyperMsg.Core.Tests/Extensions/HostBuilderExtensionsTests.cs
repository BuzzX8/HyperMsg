using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using Xunit;

namespace HyperMsg.Extensions
{
    public class HostBuilderExtensionsTests
    {
        private readonly static TimeSpan waitTimeout = TimeSpan.FromSeconds(5);

        [Fact]
        public void ConfigureObservers_Applies_Observer_Configurator()
        {
            var wasInvoked = false;
            var waitEvent = new ManualResetEventSlim();
            var builder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddCoreServices(100, 100);
                })
                .ConfigureObservers((provider, observable) =>
                {
                    wasInvoked = true;
                    waitEvent.Set();
                });

            var runTask = builder.Build().RunAsync();
            waitEvent.Wait(waitTimeout);

            Assert.True(wasInvoked);
        }

        [Fact]
        public void ConfigureObservers_Applies_ComponentObserver_Configurator()
        {
            var wasInvoked = false;
            var waitEvent = new ManualResetEventSlim();
            var builder = Host.CreateDefaultBuilder()
                .ConfigureServices(services =>
                {
                    services.AddCoreServices(100, 100);
                    services.AddSingleton<MessageBroker>();
                })
                .ConfigureObservers<MessageBroker>((component, observable) =>
                {
                    wasInvoked = true;
                    waitEvent.Set();
                });

            var runTask = builder.Build().RunAsync();
            waitEvent.Wait(waitTimeout);

            Assert.True(wasInvoked);
        }
    }
}
