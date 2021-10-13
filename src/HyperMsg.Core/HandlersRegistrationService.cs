using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Registrator = System.Func<HyperMsg.IHandlersRegistry, System.Collections.Generic.IEnumerable<System.IDisposable>>;

namespace HyperMsg
{
    public class HandlersRegistrationService : HostedServiceBase
    {
        private readonly IHandlersRegistry handlersRegistry;
        private readonly List<Registrator> registrators;
        private readonly List<IDisposable> registrations;

        public HandlersRegistrationService(HandlersRegistratorList registrators, IHandlersRegistry handlersRegistry)
        {
            this.handlersRegistry = handlersRegistry;
            registrations = new();
            this.registrators = new(registrators);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            foreach (var registrator in registrators)
            {
                registrations.AddRange(registrator.Invoke(handlersRegistry));
            }

            return base.StartAsync(cancellationToken);
        }

        public override void Dispose()
        {
            base.Dispose();
            registrations.ForEach(r => r.Dispose());
        }
    }

    public class HandlersRegistratorList : List<Registrator>
    { }
}