using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Configurator = System.Action<HyperMsg.IConfigurationContext>;

namespace HyperMsg
{
    public class ConfigurableBuilder<T> : IConfigurable
    {        
        private readonly List<Configurator> configurators;
        private readonly Dictionary<string, object> settings;
        private readonly Dictionary<Type, object> services;
                
        private Configurator currentConfigurator;
        private ConfigurationContext currentContext;
        private Queue<Configurator> configuratorQueue;

        private Dictionary<Type, ManualResetEventSlim> pendingDependencies;
        private List<Exception> configExceptions;

        public ConfigurableBuilder()
        {
            configurators = new List<Configurator>();
            settings = new Dictionary<string, object>();
            services = new Dictionary<Type, object>();
        }

        public void AddSetting(string settingName, object setting) => settings.Add(settingName, setting);

        public void Configure(Configurator configurator) => configurators.Add(configurator);

        public T Build()
        {
            currentContext = new ConfigurationContext(settings, services, ResolveService);
            configuratorQueue = new Queue<Configurator>(configurators);
            pendingDependencies = new Dictionary<Type, ManualResetEventSlim>();
            configExceptions = new List<Exception>();

            try
            {
                RunConfigurationTask().Wait();
            }
            catch (AggregateException e)
            {
                throw new Exception();
            }

            return (T)currentContext.GetService(typeof(T));
        }

        private void ResolveService(Type serviceType)
        {
            if (configuratorQueue.Count == 0)
            {
                foreach (var ev in pendingDependencies.Values)
                {
                    //ev.Set();
                }
            }

            var @event = default(ManualResetEventSlim);

            lock(pendingDependencies)
            {
                if (pendingDependencies.ContainsKey(serviceType))
                {
                    @event = pendingDependencies[serviceType];
                }
                else
                {
                    @event = new ManualResetEventSlim();
                    pendingDependencies.Add(serviceType, @event);
                }
            }

            if (configuratorQueue.Count > 0)
            {
                RunConfigurationTask().ContinueWith(t => configExceptions.Add(t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            }

            @event.Wait();

            if (!services.ContainsKey(serviceType))
            {
                throw new Exception();
            }
        }

        private Task RunConfigurationTask()
        {
            return Task.Run(() =>
            {
                while (configuratorQueue.Count > 0)
                {
                    currentConfigurator = configuratorQueue.Dequeue();
                    currentConfigurator.Invoke(currentContext);

                    var intersect = pendingDependencies.Keys.Intersect(services.Keys).ToArray();

                    if (intersect.Count() > 0)
                    {
                        foreach (var dependency in intersect)
                        {
                            var @event = pendingDependencies[dependency];
                            pendingDependencies.Remove(dependency);
                            @event.Set();
                        }
                    }
                }
            });
        }
    }
}
