﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperMsg
{
    public class ConfigurableServiceProvider : IConfigurable, IServiceProvider, IDisposable
    {
        private readonly Dictionary<string, object> settings;
        private readonly Dictionary<Type, ServiceFactory> singleInterfaceServices;
        private readonly List<(IEnumerable<Type> interfaces, ServiceFactory factory)> multiInterfaceServices;
        private readonly List<Configurator> configurators;

        private readonly Dictionary<Type, object> createdServices;
        private readonly List<IDisposable> disposables;

        private bool configuratorsInvoked = false;

        public ConfigurableServiceProvider()
        {            
            settings = new Dictionary<string, object>();
            singleInterfaceServices = new Dictionary<Type, ServiceFactory>();
            multiInterfaceServices = new List<(IEnumerable<Type> interfaces, ServiceFactory factory)>();
            configurators = new List<Configurator>();
            createdServices = new Dictionary<Type, object>();
            disposables = new List<IDisposable>();
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
            EnsureConfiguratorsRun();

            return (T)((IServiceProvider)this).GetService(typeof(T));
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            if (createdServices.ContainsKey(serviceType))
            {
                return createdServices[serviceType];
            }

            if (singleInterfaceServices.ContainsKey(serviceType))
            {
                return CreateSingleInterfaceService(serviceType);
            }

            var itemToRemove = CreateMultiInterfaceService(serviceType);

            if (itemToRemove != default)
            {
                multiInterfaceServices.Remove(itemToRemove);
                return createdServices[serviceType];
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
            foreach (var configurator in configurators)
            {
                configurator.Invoke(this, settings);
            }
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
                RegisterIfDisposable(service);

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
            RegisterIfDisposable(service);

            singleInterfaceServices.Remove(serviceInterface);
            createdServices.Add(serviceInterface, service);

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
