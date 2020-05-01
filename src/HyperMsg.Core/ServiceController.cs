using System;
using System.Collections.Generic;

namespace HyperMsg
{
    /// <summary>
    /// Provides implementation for IServiceRegistry and IServiceProvider
    /// </summary>
    public class ServiceController : IServiceRegistry, IServiceProvider, IDisposable
    {        
        private readonly Dictionary<Type, Func<IServiceProvider, object>> serviceFactories;
        private readonly Dictionary<Type, object> serviceInstances;
               
        private readonly List<IDisposable> disposables;

        public ServiceController()
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
