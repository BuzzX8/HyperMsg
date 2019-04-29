using FakeItEasy;
using System;
using Xunit;

namespace HyperMsg
{
    public class ConfigurableBuilderBaseTests
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ConfigurableBuilderImpl builder;

        public ConfigurableBuilderBaseTests()
        {
            serviceProvider = A.Fake<IServiceProvider>();
            builder = new ConfigurableBuilderImpl(d => serviceProvider);
        }

        [Fact]
        public void Build_Run_All_Configurators()
        {
            var configurators = A.CollectionOfFake<Action<Configuration>>(10);

            foreach(var configurator in configurators)
            {
                builder.Configure(configurator);
            }

            builder.Build();

            foreach (var configurator in configurators)
            {
                A.CallTo(() => configurator.Invoke(A<Configuration>._)).MustHaveHappened();
            }
        }

        private class ConfigurableBuilderImpl : ConfigurableBuilderBase<string>
        {
            public ConfigurableBuilderImpl(ServiceProviderFactory serviceProviderFactory) : base(serviceProviderFactory)
            { }

            protected override string Build(IServiceProvider serviceProvider)
            {
                return string.Empty;
            }
        }
    }
}
