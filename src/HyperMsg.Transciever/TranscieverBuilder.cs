using System;
using System.Collections.Generic;

namespace HyperMsg.Transciever
{
    public class TranscieverBuilder<T> : ITransceiverBuilder<T, T>
    {        
        private readonly List<Action<BuilderContext>> configurators;
        private readonly Func<ICollection<ServiceDescriptor>, IServiceProvider> serviceProviderFactory;

        public TranscieverBuilder(Func<ICollection<ServiceDescriptor>, IServiceProvider> serviceProviderFactory)
        {
            this.serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));
            configurators = new List<Action<BuilderContext>>();
        }

        public void Configure(Action<BuilderContext> configurator) => configurators.Add(configurator);

        public ITransceiver<T, T> Build()
        {
            var context = new BuilderContext();

            foreach (var configurator in configurators)
            {
                configurator.Invoke(context);
            }

            RegisterTransciever(context);

            var serviceProvider = serviceProviderFactory.Invoke(context.Services);
            return (ITransceiver<T, T>)serviceProvider.GetService(typeof(ITransceiver<T, T>));
        }

        private void RegisterTransciever(BuilderContext context)
        {
            var transceiver = ServiceDescriptor.Describe(typeof(ITransceiver<T, T>), typeof(MessageTransceiver<T>));
            context.Services.Add(transceiver);
        }
    }
}
