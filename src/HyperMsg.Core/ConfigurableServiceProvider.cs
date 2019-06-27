using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperMsg
{
    public class ConfigurableServiceProvider : IConfigurable, IServiceProvider
    {
        private readonly Dictionary<string, object> settings;
        private readonly Dictionary<Type, ServiceFactory> singleInterfaceServices;
        private readonly List<(IEnumerable<Type> interfaces, ServiceFactory factory)> multiInterfaceServices;
        private readonly List<Configurator> configurators;
        private readonly Dictionary<Type, object> createdServices;

        public ConfigurableServiceProvider()
        {            
            settings = new Dictionary<string, object>();
            singleInterfaceServices = new Dictionary<Type, ServiceFactory>();
            multiInterfaceServices = new List<(IEnumerable<Type> interfaces, ServiceFactory factory)>();
            configurators = new List<Configurator>();
            createdServices = new Dictionary<Type, object>();
        }

        public void AddSetting(string settingName, object setting) => settings.Add(settingName, setting);

        public void RegisterService(Type serviceIterface, ServiceFactory serviceFactory) => singleInterfaceServices.Add(serviceIterface, serviceFactory);

        public void RegisterConfigurator(Configurator configurator) => configurators.Add(configurator);

        public void RegisterService(IEnumerable<Type> serviceInterfaces, ServiceFactory serviceFactory)
        {
            multiInterfaceServices.Add((serviceInterfaces, serviceFactory));
        }

        public T GetService<T>()
        {
            foreach (var configurator in configurators)
            {
                configurator.Invoke(this, settings);
            }

            return (T)GetService(typeof(T));
        }

        public object GetService(Type serviceInterface)
        {
            if (createdServices.ContainsKey(serviceInterface))
            {
                return createdServices[serviceInterface];
            }

            if (singleInterfaceServices.ContainsKey(serviceInterface))
            {
                return CreateSingleInterfaceService(serviceInterface);
            }

            var itemToRemove = CreateMultiInterfaceService(serviceInterface);

            if (itemToRemove != default)
            {
                multiInterfaceServices.Remove(itemToRemove);
                return createdServices[serviceInterface];
            }

            throw new InvalidOperationException($"Can not resolve service for interface {serviceInterface}");
        }

        private (IEnumerable<Type> interfaces, ServiceFactory factory) CreateMultiInterfaceService(Type serviceInterface)
        {
            var itemToRemove = default((IEnumerable<Type> interfaces, ServiceFactory factory));

            foreach (var tuple in multiInterfaceServices)
            {
                (var interfaces, var factory) = tuple;

                if (!interfaces.Contains(serviceInterface))
                {
                    continue;
                }

                var service = factory.Invoke(this, settings);

                foreach (var @interface in interfaces)
                {
                    createdServices.Add(@interface, service);
                }

                itemToRemove = tuple;
                break;
            }

            return itemToRemove;
        }

        private object CreateSingleInterfaceService(Type serviceInterface)
        {
            var factory = singleInterfaceServices[serviceInterface];
            var service = factory.Invoke(this, settings);

            singleInterfaceServices.Remove(serviceInterface);
            createdServices.Add(serviceInterface, service);

            return service;
        }
    }
}
