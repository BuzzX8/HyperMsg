using FakeItEasy;
using System;
using Xunit;

namespace HyperMsg
{
    public class ConfigurableBuilderTests
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ConfigurableBuilder<string> builder;

        public ConfigurableBuilderTests()
        {
            serviceProvider = A.Fake<IServiceProvider>();
            A.CallTo(() => serviceProvider.GetService(typeof(string))).Returns(string.Empty);
            builder = new ConfigurableBuilder<string>(d => serviceProvider);
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
    }
}
