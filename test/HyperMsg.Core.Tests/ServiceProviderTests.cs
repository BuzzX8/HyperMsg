using FakeItEasy;
using System;
using Xunit;
using ServiceFactory = System.Func<System.IServiceProvider, object>;

namespace HyperMsg
{
    public class ServiceProviderTests
    {
        private readonly ServiceProvider provider = new ServiceProvider();

        [Fact]
        public void GetService_Does_Not_Invokes_ServiceFactory_If_It_Not_Required()
        {
            var factory = A.Fake<ServiceFactory>();
            provider.AddService(typeof(string), (p) => string.Empty);

            provider.GetService<string>();

            A.CallTo(() => factory.Invoke(A<IServiceProvider>._)).MustNotHaveHappened();
        }

        [Fact]
        public void GetService_Returns_Registered_Service()
        {
            var expected = Guid.NewGuid().ToString();
            var factory = A.Fake<ServiceFactory>();
            A.CallTo(() => factory.Invoke(A<IServiceProvider>._)).Returns(expected);
            provider.AddService(typeof(string), factory);

            var actual = provider.GetService<string>();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetService_Resolves_Complex_Dependencies()
        {
            var expected = Guid.NewGuid().ToString();            
            provider.AddService(typeof(IMessageSender), (p) => A.Fake<IMessageSender>());
            provider.AddService(typeof(IMessageObservable), (p) => A.Fake<IMessageObservable>());
            provider.AddService(typeof(string), (p) =>
            {
                Assert.NotNull(p.GetService(typeof(IMessageSender)) as IMessageSender);
                Assert.NotNull(p.GetService(typeof(IMessageObservable)) as IMessageObservable);
                return expected;
            });

            var actual = provider.GetService<string>();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetService_Throws_Exception_If_No_Registered_Dependencies()
        {
            var expected = Guid.NewGuid().ToString();
            provider.AddCoreServices(100, 100);

            Assert.Throws<InvalidOperationException>(() => provider.GetService<string>());
        }

        [Fact]
        public void GetService_Rethrows_Exception_Thrown_By_Factory()
        {
            provider.AddService(typeof(Guid), (p) => throw new ArgumentNullException());
            provider.AddService(typeof(string), (p) =>
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
            provider.AddService(typeof(IBufferReader<byte>), (p) => service);
            provider.GetService<IBufferReader<byte>>();

            provider.Dispose();

            A.CallTo(() => ((IDisposable)service).Dispose()).MustHaveHappened();
        }

        [Fact]
        public void GetService_Invokes_All_Configurator_By_Other_Configurators()
        {
            var innerConfigurator = A.Fake<Action<IServiceProvider>>();
            provider.AddService(typeof(Guid), (p) => Guid.NewGuid());
            provider.AddInitializer((p) =>
            {
                provider.AddInitializer(innerConfigurator);
            });

            provider.GetService<Guid>();

            A.CallTo(() => innerConfigurator.Invoke(A<IServiceProvider>._)).MustHaveHappened();
        }
    }
}
