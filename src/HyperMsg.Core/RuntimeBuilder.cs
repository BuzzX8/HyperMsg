namespace HyperMsg
{
    public class RuntimeBuilder
    {
        class ServiceProvider : IServiceProvider
        {
            private readonly Dictionary<Type, object> services;

            internal ServiceProvider(Dictionary<Type, object> services) => this.services = new(services);

            public object GetService(Type serviceType)
            {
                if (!services.ContainsKey(serviceType))
                {
                    return null;
                }

                return services[serviceType];
            }
        }

        private readonly Dictionary<Type, object> services = new();
        private Action<IRuntime> initializers;

        public IRuntime Build()
        {
            var provider = new ServiceProvider(services);
            var runtime = new Runtime(provider);

            initializers?.Invoke(runtime);

            return runtime;
        }

        public void AddService<T>(T service) => services[typeof(T)] = service;

        public void AddInitializer(Action<IRuntime> initializer) => initializers += initializer;
    }
}
