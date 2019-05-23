using FakeItEasy;
using System;
using Xunit;

namespace HyperMsg
{
    public class ConfigurableBuilderTests
    {
        [Fact]
        public void Build_Invokes_All_Configurators()
        {
            var builder = new ConfigurableBuilder<string>();
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
            var builder = new ConfigurableBuilder<string>();
            var expected = Guid.NewGuid().ToString();
            builder.Configure(c =>
            {
                c.RegisterService(typeof(string), expected);
            });

            var actual = builder.Build();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConfigurationContext_GetSetting_Returns_Previuosly_Added_Setting()
        {
            var builder = new ConfigurableBuilder<string>();
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;
            builder.AddSetting(nameof(Guid), expected);
            builder.Configure(c =>
            {
                actual = (Guid)c.GetSetting(nameof(Guid));
                c.RegisterService(typeof(string), "");
            });

            builder.Build();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Build_Resolves_Direct_Dependency_Registered_Before_Service()
        {
            var builder = new ConfigurableBuilder<IBufferReader>();
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;

            builder.Configure(c => c.RegisterService(typeof(IStream), A.Fake<IStream>()));
            builder.UseBufferReader(100);

            var reader = builder.Build();

            Assert.NotNull(reader);
        }

        [Fact]
        public void Build_Resolves_Direct_Dependency_Registered_After_Service()
        {
            var builder = new ConfigurableBuilder<IBufferReader>();
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;

            builder.UseBufferReader(100);
            builder.Configure(c => c.RegisterService(typeof(IStream), A.Fake<IStream>()));            

            var reader = builder.Build();

            Assert.NotNull(reader);
        }

        [Fact]
        public void Build_Resolves_Complex_Dependencies()
        {
            var builder = new ConfigurableBuilder<string>();
            var expected = Guid.NewGuid().ToString();
            builder.UseCoreServices<Guid>(100, 100);
            builder.Configure(context =>
            {
                context.RegisterService(typeof(ISerializer<Guid>), A.Fake<ISerializer<Guid>>());
                context.RegisterService(typeof(IStream), A.Fake<IStream>());
                context.RegisterService(typeof(IHandler<TransportCommands>), A.Fake<IHandler<TransportCommands>>());
            });
            builder.Configure(context =>
            {
                var transceiver = (ITransceiver<Guid, Guid>)context.GetService(typeof(ITransceiver<Guid, Guid>));
                context.RegisterService(typeof(string), expected);
            });

            var actual = builder.Build();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Build_Throws_Exception_If_No_Registered_Dependencies()
        {
            var builder = new ConfigurableBuilder<string>();
            var expected = Guid.NewGuid().ToString();
            builder.UseCoreServices<Guid>(100, 100);
            builder.Configure(context =>
            {
                var transceiver = (ITransceiver<Guid, Guid>)context.GetService(typeof(ITransceiver<Guid, Guid>));
                context.RegisterService(typeof(string), expected);
            });

            Assert.Throws<Exception>(() => builder.Build());
        }
    }
}
