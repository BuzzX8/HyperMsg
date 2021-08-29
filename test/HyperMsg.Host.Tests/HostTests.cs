using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class HostTests
    {
        private readonly CancellationTokenSource tokenSource;
        private readonly Host host;
        private readonly List<IHostedService> hostedServices;

        public HostTests()
        {
            var services = new ServiceCollection();
            var hostedService1 = A.Fake<HostedService1>();
            var hostedService2 = A.Fake<HostedService2>();
            hostedServices = new()
            {
                hostedService1,
                hostedService2
            };
            services.AddHostedService(provider => hostedService1);
            services.AddHostedService(provider => hostedService2);
            host = new(services);
            tokenSource = new();
        }

        [Fact]
        public async Task StartAsync_Starts_All_Hosted_Services()
        {
            await host.StartAsync(tokenSource.Token);

            hostedServices.ForEach(s => A.CallTo(() => s.StartAsync(tokenSource.Token)).MustHaveHappened());
        }

        [Fact]
        public async Task StoptAsync_Stops_All_Hosted_Services()
        {
            await host.StopAsync(tokenSource.Token);

            hostedServices.ForEach(s => A.CallTo(() => s.StopAsync(tokenSource.Token)).MustHaveHappened());
        }

        [Fact]
        public void CreateDefault_Invokes_Provided_Configurator()
        {
            var configurator = A.Fake<Action<IServiceCollection>>();

            Host.CreateDefault(configurator);

            A.CallTo(() => configurator.Invoke(A<IServiceCollection>._)).MustHaveHappened();
        }

        [Fact]
        public void CreateDefault_Adds_MessagingServices()
        {
            var host = Host.CreateDefault();

            var sender = host.GetRequiredService<ISender>();

            Assert.NotNull(sender);
        }

        public abstract class HostedService1 : IHostedService
        {
            public abstract Task StartAsync(CancellationToken cancellationToken);
            public abstract Task StopAsync(CancellationToken cancellationToken);
        }

        public abstract class HostedService2 : IHostedService
        {
            public abstract Task StartAsync(CancellationToken cancellationToken);
            public abstract Task StopAsync(CancellationToken cancellationToken);
        }
    }
}
