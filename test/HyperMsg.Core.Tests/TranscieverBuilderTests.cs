using FakeItEasy;
using System;
using Xunit;

namespace HyperMsg.Transciever
{
    public class TranscieverBuilderTests
    {
        [Fact(Skip = "Not implemented")]
        public void Build_Run_All_Configurators()
        {
            var serviceProvider = A.Fake<IServiceProvider>();
            A.CallTo(() => serviceProvider.GetService(typeof(IPipeWriter))).Returns(A.Fake<IPipeWriter>());
            A.CallTo(() => serviceProvider.GetService(typeof(ISerializer<Guid>))).Returns(A.Fake<ISerializer<Guid>>());
            A.CallTo(() => serviceProvider.GetService(typeof(ITransceiver<Guid, Guid>))).Returns(A.Fake<ITransceiver<Guid, Guid>>());
            var builder = new TranscieverBuilder<Guid>(d => serviceProvider);

            var configurators = A.CollectionOfFake<Action<BuilderContext>>(10);

            foreach(var configurator in configurators)
            {
                builder.Configure(configurator);
            }

            builder.Build();

            foreach (var configurator in configurators)
            {
                A.CallTo(() => configurator.Invoke(A<BuilderContext>._)).MustHaveHappened();
            }
        }
    }
}
