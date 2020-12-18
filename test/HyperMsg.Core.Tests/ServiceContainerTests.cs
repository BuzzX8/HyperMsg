using FakeItEasy;
using HyperMsg.Extensions;
using System;
using Xunit;
using ServiceFactory = System.Func<System.IServiceProvider, object>;

namespace HyperMsg
{
    public class ServiceContainerTests
    {
        private readonly ServiceContainer container = new ServiceContainer();

        [Fact]
        public void GetService_Does_Not_Invokes_ServiceFactory_If_It_Not_Required()
        {
            var factory = A.Fake<ServiceFactory>();
            container.Add(typeof(string), (p) => string.Empty);

            container.GetService<string>();

            A.CallTo(() => factory.Invoke(A<IServiceProvider>._)).MustNotHaveHappened();
        }

        [Fact]
        public void GetService_Returns_Registered_Service()
        {
            var expected = Guid.NewGuid().ToString();
            var factory = A.Fake<ServiceFactory>();
            A.CallTo(() => factory.Invoke(A<IServiceProvider>._)).Returns(expected);
            container.Add(typeof(string), factory);

            var actual = container.GetService<string>();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetService_Resolves_Complex_Dependencies()
        {
            var expected = Guid.NewGuid().ToString();            
            container.Add(typeof(IMessageSender), (p) => A.Fake<IMessageSender>());
            container.Add(typeof(IMessageObservable), (p) => A.Fake<IMessageObservable>());
            container.Add(typeof(string), (p) =>
            {
                Assert.NotNull(p.GetService(typeof(IMessageSender)) as IMessageSender);
                Assert.NotNull(p.GetService(typeof(IMessageObservable)) as IMessageObservable);
                return expected;
            });

            var actual = container.GetService<string>();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetService_Throws_Exception_If_No_Registered_Dependencies()
        {
            var expected = Guid.NewGuid().ToString();
            container.AddCoreServices(100, 100);

            Assert.Throws<InvalidOperationException>(() => container.GetService<string>());
        }

        [Fact]
        public void GetService_Rethrows_Exception_Thrown_By_Factory()
        {
            container.Add(typeof(Guid), (p) => throw new ArgumentNullException());
            container.Add(typeof(string), (p) =>
            {
                p.GetService(typeof(Guid));
                return string.Empty;
            });

            Assert.Throws<ArgumentNullException>(() => container.GetService<string>());
        }

        [Fact]
        public void GetService_Builds_Services()
        {
            var factory = A.Fake<ServiceFactory>();
            container.Add(typeof(int), 0);
            container.Add(typeof(object), factory);

            container.GetService<int>();

            A.CallTo(() => factory.Invoke(A<IServiceProvider>._)).MustHaveHappened();
        }

        [Fact]
        public void Dispose_Disposes_Disposable_Services()
        {
            var service = A.Fake<IBufferReader<byte>>(o =>
            {
                o.Implements<IDisposable>();
            });
            container.Add(typeof(IBufferReader<byte>), (p) => service);
            container.GetService<IBufferReader<byte>>();

            container.Dispose();

            A.CallTo(() => ((IDisposable)service).Dispose()).MustHaveHappened();
        }
    }
}
