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
            builder.Configure(c => c.RegisterService(typeof(string), ""));

            foreach (var configurator in configurators)
            {
                builder.Configure(configurator);
            }

            builder.Build();

            foreach (var configurator in configurators)
            {
                A.CallTo(() => configurator.Invoke(A<IConfigurationContext>._)).MustHaveHappened();
            }
        }

        [Fact]
        public void Build_Returns_Registered_Service()
        {
            var expected = Guid.NewGuid().ToString();
            builder.Configure(c =>
            {
                c.RegisterService(typeof(string), expected);
            });

            var actual = builder.Build();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConfigurationContext_GetService_Returns_Previously_Registered_Service()
        {
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;
            builder.Configure(c => c.RegisterService(typeof(Guid), expected));

            builder.Configure(c =>
            {
                actual = (Guid)c.GetService(typeof(Guid));
                c.RegisterService(typeof(string), "");
            });

            builder.Build();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConfigurationContext_GetService_Returns_Service_Registered_After()
        {
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;

            builder.Configure(c =>
            {
                actual = (Guid)c.GetService(typeof(Guid));
                c.RegisterService(typeof(string), "");
            });

            builder.Configure(c => c.RegisterService(typeof(Guid), expected));            

            builder.Build();

            Assert.Equal(expected, actual);
        }
    }
}
