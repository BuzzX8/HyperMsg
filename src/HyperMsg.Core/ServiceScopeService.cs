using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class ServiceScopeService : MessagingService
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
            yield return RegisterHandler<StartServiceScope>(command =>
            {
                var serviceHost = ServiceHost.CreateDefault(services =>
                {
                    command.ServiceConfigurator.Invoke(services);
                });

                lock(sync)
                {
                    serviceHosts.Add(serviceHost);
                }

                command.DisposeHandle = new ServiceHostDisposeHandle(this, serviceHost);
            });
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }

        private class ServiceHostDisposeHandle : IDisposable
        {
            private readonly ServiceScopeService scopeService;
            private readonly ServiceHost serviceHost;

            internal ServiceHostDisposeHandle(ServiceScopeService scopeService, ServiceHost serviceHost)
            {
                this.scopeService = scopeService;
                this.serviceHost = serviceHost;
            }

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

    internal class StartServiceScope
    {
        internal IDisposable DisposeHandle { get; set; }

        internal Action<IServiceCollection> ServiceConfigurator { get; set; }
    }
}
