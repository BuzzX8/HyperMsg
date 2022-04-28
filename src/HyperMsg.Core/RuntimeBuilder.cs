using Microsoft.Extensions.DependencyInjection;

namespace HyperMsg
{
    public class RuntimeBuilder
    {
        private Action<IRuntime> initializers;

        public IRuntime Build()
        {
            var provider = Services.BuildServiceProvider();
            var runtime = new Runtime(provider);

            initializers?.Invoke(runtime);

            return runtime;
        }

        public IServiceCollection Services { get; } = new ServiceCollection();

        public void AddInitializer(Action<IRuntime> initializer) => initializers += initializer;
    }
}
