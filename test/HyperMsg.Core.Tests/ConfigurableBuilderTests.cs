using FakeItEasy;
using System;
using System.Collections.Generic;
using Xunit;

namespace HyperMsg
{
    public class ConfigurableBuilderTests
    {
        [Fact]
        public void Build_Does_Not_Invokes_ServiceFactory_If_It_Not_Required()
        {
            var builder = new ConfigurableBuilder<string>();
            var factory = A.Fake<ServiceFactory>();
            builder.AddService(typeof(string), (p, s) => string.Empty);

            builder.Build();

            A.CallTo(() => factory.Invoke(A<IServiceProvider>._, A<IReadOnlyDictionary<string, object>>._)).MustNotHaveHappened();
        }

        [Fact]
        public void Build_Returns_Registered_Service()
        {
            var builder = new ConfigurableBuilder<string>();
            var expected = Guid.NewGuid().ToString();
            var factory = A.Fake<ServiceFactory>();
            A.CallTo(() => factory.Invoke(A<IServiceProvider>._, A<IReadOnlyDictionary<string, object>>._)).Returns(expected);
            builder.AddService(typeof(string), factory);

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
            builder.AddService(typeof(string), (p, s) =>
            {
                actual = (Guid)s[nameof(Guid)];
                return string.Empty;
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

            builder.AddService(typeof(IStream), (p, s) => A.Fake<IStream>());
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
            builder.AddService(typeof(IStream), (p, s) => A.Fake<IStream>());

            var reader = builder.Build();

            Assert.NotNull(reader);
        }

        [Fact]
        public void Build_Resolves_Complex_Dependencies()
        {
            var builder = new ConfigurableBuilder<string>();
            var expected = Guid.NewGuid().ToString();
            builder.UseCoreServices<Guid>(100, 100);
            builder.AddService(typeof(ISerializer<Guid>), (p, s) => A.Fake<ISerializer<Guid>>());
            builder.AddService(typeof(IStream), (p, s) => A.Fake<IStream>());
            builder.AddService(typeof(IHandler<TransportOperations>), (p, s) => A.Fake<IHandler<TransportOperations>>());

            builder.AddService(typeof(string), (p, s) =>
            {
                var transceiver = (ITransceiver<Guid, Guid>)p.GetService(typeof(ITransceiver<Guid, Guid>));
                return expected;
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
            builder.AddService(typeof(string), (p, s) =>
            {
                var transceiver = (ITransceiver<Guid, Guid>)p.GetService(typeof(ITransceiver<Guid, Guid>));
                return expected;
            });

            Assert.Throws<InvalidOperationException>(() => builder.Build());
        }

        [Fact]
        public void Builder_Rethrows_Exception_Thrown_By_Factory()
        {
            var builder = new ConfigurableBuilder<string>();
            builder.AddService(typeof(Guid), (p, s) => throw new ArgumentNullException());
            builder.AddService(typeof(string), (p, s) =>
            {
                p.GetService(typeof(Guid));
                return string.Empty;
            });

            Assert.Throws<ArgumentNullException>(() => builder.Build());
        }
    }
}
