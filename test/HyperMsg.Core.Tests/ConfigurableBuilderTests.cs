using FakeItEasy;
using System;
using Xunit;

namespace HyperMsg
{
    public class ConfigurableBuilderTests
    {        
        private readonly ConfigurableBuilder<string> builder;

        public ConfigurableBuilderTests()
        {
            builder = new ConfigurableBuilder<string>();
        }

        [Fact]
        public void Build_Invokes_Configurators()
        {
            var configurators = A.CollectionOfFake<Action<IConfigurationContext>>(10);

            foreach(var configurator in configurators)
            {
                builder.Configure(configurator);
            }

            builder.Build();

            foreach (var configurator in configurators)
            {
                A.CallTo(() => configurator.Invoke(A<IConfigurationContext>._)).MustHaveHappened();
            }
        }
    }
}
