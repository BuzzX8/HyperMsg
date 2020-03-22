using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperMsg
{
    /// <summary>
    /// Provides implementation for IConfigurable and IServiceProvider
    /// </summary>
    public class ConfigurableServiceProvider : IConfigurable, IServiceProvider, IDisposable
    {        
        private readonly Dictionary<Type, Func<IServiceProvider, object>> serviceFactories;
        private readonly Dictionary<Type, object> serviceInstances;
        private readonly List<Action<IServiceProvider>> initializers;
               
        private readonly List<IDisposable> disposables;

        private bool configuratorsInvoked = false;

        public ConfigurableServiceProvider()
        {
            serviceFactories = new Dictionary<Type, Func<IServiceProvider, object>>();
            serviceInstances = new Dictionary<Type, object>();
            initializers = new List<Action<IServiceProvider>>();
            disposables = new List<IDisposable>();
        }

        public void AddInitializer(Action<IServiceProvider> initializer) => initializers.Add(initializer);

        public void AddService(Type serviceType, object serviceInstance)
        {
            serviceInstances.Add(serviceType, serviceInstance);
            RegisterIfDisposable(serviceInstance);
        }

        public void AddService(Type serviceType, Func<IServiceProvider, object> serviceFactory)
        {
            serviceFactories.Add(serviceType, serviceFactory);
        }

        /// <summary>
        /// Returns previously registered service.
        /// </summary>
        /// <typeparam name="T">Type of service.</typeparam>
        /// <exception cref="InvalidOperationException">
        /// Rises when requested service type was not previously registred.
        /// </exception>
        /// <returns>Implementation for service.</returns>
        public T GetService<T>()
        {
            EnsureConfiguratorsRun();

            return (T)((IServiceProvider)this).GetService(typeof(T));
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            if (serviceInstances.ContainsKey(serviceType))
            {
                return serviceInstances[serviceType];
            }

            if (serviceFactories.ContainsKey(serviceType))
            {
                return CreateService(serviceType);
            }

            throw new InvalidOperationException($"Can not resolve service for interface {serviceType}");
        }

        private void EnsureConfiguratorsRun()
        {
            if (!configuratorsInvoked)
            {
                RunConfigurators();
                configuratorsInvoked = true;
            }
        }

        private void RunConfigurators()
        {
            while (initializers.Any())
            {
                var currentinitializers = initializers.ToArray();
                initializers.Clear();
                foreach (var initializer in currentinitializers)
                {
                    initializer.Invoke(this);
                }
            }
        }

        private object CreateService(Type serviceType)
        {
            var factory = serviceFactories[serviceType];
            var service = factory.Invoke(this);
            RegisterIfDisposable(service);

            serviceFactories.Remove(serviceType);
            serviceInstances.Add(serviceType, service);

            return service;
        }

        private void RegisterIfDisposable(object service)
        {
            if (service is IDisposable)
            {
                disposables.Add((IDisposable)service);
            }
        }

        public void Dispose() => disposables.ForEach(d => d.Dispose());
    }
}
