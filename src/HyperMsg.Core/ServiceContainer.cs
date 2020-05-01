using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperMsg
{
    /// <summary>
    /// Provides implementation for IServiceRegistry and IServiceProvider
    /// </summary>
    public class ServiceContainer : IServiceRegistry, IServiceProvider, IDisposable
    {        
        private readonly Dictionary<Type, Func<IServiceProvider, object>> serviceFactories;
        private readonly Dictionary<Type, object> serviceInstances;
               
        private readonly List<IDisposable> disposables;

        public ServiceContainer()
        {
            serviceFactories = new Dictionary<Type, Func<IServiceProvider, object>>();
            serviceInstances = new Dictionary<Type, object>();
            disposables = new List<IDisposable>();
        }

        public void Add(Type serviceType, object serviceInstance)
        {
            serviceInstances.Add(serviceType, serviceInstance);
            RegisterIfDisposable(serviceInstance);
        }

        public void Add(Type serviceType, Func<IServiceProvider, object> serviceFactory)
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
        public T GetService<T>() => (T)((IServiceProvider)this).GetService(typeof(T));

        object IServiceProvider.GetService(Type serviceType)
        {
            BuildServices();
            if (serviceInstances.ContainsKey(serviceType))
            {
                return serviceInstances[serviceType];
            }

            throw new InvalidOperationException($"Can not resolve service for interface {serviceType}");
        }

        private void BuildServices()
        {
            if (serviceFactories.Count == 0)
            {
                return;
            }

            var serviceTypes = serviceFactories.Keys.ToArray();

            for(int i = 0; i < serviceTypes.Length; i++)
            {
                CreateService(serviceTypes[i]);
            }
        }

        private void CreateService(Type serviceType)
        {
            var factory = serviceFactories[serviceType];
            var service = factory.Invoke(this);
            RegisterIfDisposable(service);

            serviceFactories.Remove(serviceType);
            serviceInstances.Add(serviceType, service);
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
