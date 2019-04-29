using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public abstract class ConfigurableBuilderBase<T> : IConfigurableBuilder<T>
    {        
        private readonly List<Action<BuilderContext>> configurators;
        private readonly ServiceProviderFactory serviceProviderFactory;

        public ConfigurableBuilderBase(ServiceProviderFactory serviceProviderFactory)
        {
            this.serviceProviderFactory = serviceProviderFactory ?? throw new ArgumentNullException(nameof(serviceProviderFactory));
            configurators = new List<Action<BuilderContext>>();
        }

        public void Configure(Action<BuilderContext> configurator) => configurators.Add(configurator);

        public T Build()
        {
            var context = new BuilderContext(null);

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
