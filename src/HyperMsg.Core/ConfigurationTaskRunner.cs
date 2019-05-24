using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperMsg
{
    internal class ConfigurationTaskRunner
    {
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
        private readonly Dictionary<Type, List<TaskCompletionSource<object>>> pendingDependencies = new Dictionary<Type, List<TaskCompletionSource<object>>>();
        private readonly List<Action<IConfigurationContext>> configurators;
        private List<Exception> configurationExceptions;
        private readonly object sync = new object();
        private int activeTaskCount;
        private int pendingCount;

        internal ConfigurationTaskRunner(List<Action<IConfigurationContext>> configurators)
        {
            this.configurators = configurators;            
        }

        internal T GetService<T>() => (T)services[typeof(T)];

        internal Task<object> ResolveDependencyAsync(Type type)
        {
            lock(sync)
            {
                if (services.ContainsKey(type))
                {
                    return Task.FromResult(services[type]);
                }

                var tsc = new TaskCompletionSource<object>();
                
                
                if(!pendingDependencies.ContainsKey(type))
                {
                    pendingDependencies.Add(type, new List<TaskCompletionSource<object>>());
                }

                pendingDependencies[type].Add(tsc);
                OnPendingTask();
                return tsc.Task;
            }
        }

        internal void RegisterDependency(Type type, object service)
        {
            lock (sync)
            {
                services.Add(type, service);

                if (pendingDependencies.ContainsKey(type))
                {
                    var pendings = pendingDependencies[type];

                    foreach (var pending in pendings)
                    {
                        OnReativatedTask();
                        pending.SetResult(service);
                    }

                    pendingDependencies.Remove(type);
                }
            }
        }

        internal void RunConfiguration(Dictionary<string, object> settings)
        {
            activeTaskCount = configurators.Count;
            var context = new ConfigurationContext(settings, this);
            configurationExceptions = new List<Exception>();
            var tasks = configurators.Select(c => Task.Run(() => c(context)).ContinueWith(OnConfigurationTaskCompleted)).ToArray();

            Task.WaitAll(tasks);

            if (pendingCount > 0)
            {
                throw new InvalidOperationException();
            }

            if (configurationExceptions.Count > 0)
            {
                throw new AggregateException(configurationExceptions);
            }
        }

        private void OnConfigurationTaskCompleted(Task task)
        {
            lock (sync)
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    activeTaskCount--;
                }

                if (task.Status == TaskStatus.Faulted)
                {
                    configurationExceptions.Add(task.Exception);
                }
            }
        }

        private void OnPendingTask()
        {
            lock (sync)
            {
                activeTaskCount--;
                pendingCount++;

                if (activeTaskCount == 0)
                {
                    foreach (var pending in pendingDependencies.SelectMany(p => p.Value))
                    {
                        pending.SetCanceled();
                    }

                    pendingDependencies.Clear();                    
                }
            }
        }

        private void OnReativatedTask()
        {
            lock (sync)
            {
                pendingCount--;
                activeTaskCount++;
            }
        }
    }
}
