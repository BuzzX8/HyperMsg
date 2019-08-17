using FakeItEasy;
using System;
using System.Collections.Generic;
using Xunit;

namespace HyperMsg
{
    public class ConfigurableServiceProviderTests
    {
        private readonly ConfigurableServiceProvider provider = new ConfigurableServiceProvider();

        [Fact]
        public void GetService_Does_Not_Invokes_ServiceFactory_If_It_Not_Required()
        {
            var factory = A.Fake<ServiceFactory>();
            provider.RegisterService(typeof(string), (p, s) => string.Empty);

            provider.GetService<string>();

            A.CallTo(() => factory.Invoke(A<IServiceProvider>._, A<IReadOnlyDictionary<string, object>>._)).MustNotHaveHappened();
        }

        [Fact]
        public void GetService_Returns_Registered_Service()
        {
            var expected = Guid.NewGuid().ToString();
            var factory = A.Fake<ServiceFactory>();
            A.CallTo(() => factory.Invoke(A<IServiceProvider>._, A<IReadOnlyDictionary<string, object>>._)).Returns(expected);
            provider.RegisterService(typeof(string), factory);

            var actual = provider.GetService<string>();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ConfigurationContext_GetSetting_Returns_Previuosly_Added_Setting()
        {
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;
            provider.AddSetting(nameof(Guid), expected);
            provider.RegisterService(typeof(string), (p, s) =>
            {
                actual = (Guid)s[nameof(Guid)];
                return string.Empty;
            });

            provider.GetService<string>();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetService_Resolves_Direct_Dependency_Registered_Before_Service()
        {
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;

            provider.RegisterService(typeof(ITransport), (p, s) => A.Fake<ITransport>());
            provider.RegisterService(typeof(IBufferReader<byte>), (p, s) => A.Fake<IBufferReader<byte>>());

            var reader = provider.GetService<IBufferReader<byte>>();

            Assert.NotNull(reader);
        }

        [Fact]
        public void GetService_Resolves_Direct_Dependency_Registered_After_Service()
        {
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;

            provider.RegisterService(typeof(IBufferReader<byte>), (p, s) => A.Fake<IBufferReader<byte>>());
            provider.RegisterService(typeof(ITransport), (p, s) => A.Fake<ITransport>());

            var reader = provider.GetService<IBufferReader<byte>>();

            Assert.NotNull(reader);
        }

        [Fact]
        public void GetService_Resolves_Complex_Dependencies()
        {
            var expected = Guid.NewGuid().ToString();            
            provider.RegisterService(typeof(ISerializer<Guid>), (p, s) => A.Fake<ISerializer<Guid>>());
            provider.RegisterService(typeof(ITransport), (p, s) => A.Fake<ITransport>());
            provider.RegisterService(typeof(string), (p, s) =>
            {
                Assert.NotNull(p.GetService(typeof(ISerializer<Guid>)) as ISerializer<Guid>);
                Assert.NotNull(p.GetService(typeof(ITransport)) as ITransport);
                return expected;
            });

            var actual = provider.GetService<string>();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetService_Throws_Exception_If_No_Registered_Dependencies()
        {
            var expected = Guid.NewGuid().ToString();
            provider.UseCoreServices<Guid>(100, 100);

            Assert.Throws<InvalidOperationException>(() => provider.GetService<string>());
        }

        [Fact]
        public void Builder_Rethrows_Exception_Thrown_By_Factory()
        {
            provider.RegisterService(typeof(Guid), (p, s) => throw new ArgumentNullException());
            provider.RegisterService(typeof(string), (p, s) =>
            {
                p.GetService(typeof(Guid));
                return string.Empty;
            });

            Assert.Throws<ArgumentNullException>(() => provider.GetService<string>());
        }

        [Fact]
        public void Dispose_Disposes_Disposable_Services()
        {
            var service = A.Fake<IBufferReader<byte>>(o =>
            {
                o.Implements<IDisposable>();
            });
            provider.RegisterService(typeof(IBufferReader<byte>), (p, s) => service);
            provider.GetService<IBufferReader<byte>>();

            provider.Dispose();

            A.CallTo(() => ((IDisposable)service).Dispose()).MustHaveHappened();
        }
    }
}
