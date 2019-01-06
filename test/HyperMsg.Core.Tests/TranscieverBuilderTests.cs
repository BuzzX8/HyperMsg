using FakeItEasy;
using HyperMsg.Transciever;
using System;
using Xunit;

namespace HyperMsg
{
    public class TranscieverBuilderTests
    {
        [Fact]
        public void Build_Run_All_Configurators()
        {
            var serviceProvider = A.Fake<IServiceProvider>();
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
