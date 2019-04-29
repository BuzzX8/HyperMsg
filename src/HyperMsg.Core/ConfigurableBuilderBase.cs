using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public abstract class ConfigurableBuilderBase<T> : IConfigurable
    {        
        private readonly List<Action<Configuration>> configurators;
        private readonly ServiceProviderFactory serviceProviderFactory;

        public ConfigurableBuilderBase(ServiceProviderFactory serviceProviderFactory)
        {
            this.serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));
            configurators = new List<Action<Configuration>>();
        }

        public void Configure(Action<Configuration> configurator) => configurators.Add(configurator);

        public T Build()
        {
            var context = new Configuration();

            foreach (var configurator in configurators)
            {
                configurator.Invoke(context);
            }            

            var serviceProvider = serviceProviderFactory.Invoke(context.Services);

            return Build(serviceProvider);
        }

        protected abstract T Build(IServiceProvider serviceProvider);
    }
}
