using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    internal class ServiceScopeService : MessagingService
    {
        internal readonly List<ServiceHost> serviceHosts;
        internal readonly object sync;

        public ServiceScopeService(IMessagingContext messagingContext) : base(messagingContext)
        {
            serviceHosts = new();
            sync = new();
        }

        protected override IEnumerable<IDisposable> GetDefaultDisposables()
        {
            yield return RegisterHandler<StartServiceScopeRequest>(async (command, token) =>
            {
                var serviceHost = ServiceHost.CreateDefault(services =>
                {
                    command.ServiceConfigurator.Invoke(services);
                });

                await serviceHost.StartAsync(token);

                lock(sync)
                {
                    serviceHosts.Add(serviceHost);
                }

                command.ServiceScope = new ServiceScope(this, serviceHost);
            });
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach(var host in serviceHosts)
            {
                await host.StopAsync(cancellationToken);
                host.Dispose();
            }

            serviceHosts.Clear();
        }

        private class ServiceScope : IServiceScope
        {
            private readonly ServiceScopeService scopeService;
            private readonly ServiceHost serviceHost;

            internal ServiceScope(ServiceScopeService scopeService, ServiceHost serviceHost)
            {
                this.scopeService = scopeService;
                this.serviceHost = serviceHost;
            }

            public IServiceProvider ServiceProvider => serviceHost;

            public void Dispose()
            {
                lock(scopeService.sync)
                {
                    serviceHost.Stop();
                    serviceHost.Dispose();
                    scopeService.serviceHosts.Remove(serviceHost);
                }
            }
        }
    }

    internal class StartServiceScopeRequest
    {
        internal IServiceScope ServiceScope { get; set; }

        internal Action<IServiceCollection> ServiceConfigurator { get; set; }
    }
}
