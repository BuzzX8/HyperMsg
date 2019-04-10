using FakeItEasy;
using System;
using Xunit;

namespace HyperMsg
{
    public class TranscieverBuilderTests
    {
        private readonly IServiceProvider serviceProvider;
        private readonly ISerializer<Guid> serializer;
        private readonly IStream stream;
        private readonly TranscieverBuilder<Guid> transcieverBuilder;

        public TranscieverBuilderTests()
        {
            serviceProvider = A.Fake<IServiceProvider>();
            serializer = A.Fake<ISerializer<Guid>>();
            stream = A.Fake<IStream>();
            A.CallTo(() => serviceProvider.GetService(typeof(ISerializer<Guid>))).Returns(serializer);
            A.CallTo(() => serviceProvider.GetService(typeof(IStream))).Returns(stream);
            transcieverBuilder = new TranscieverBuilder<Guid>(d => serviceProvider);
        }

        [Fact]
        public void Build_Run_All_Configurators()
        {
            var configurators = A.CollectionOfFake<Action<BuilderContext>>(10);

            foreach(var configurator in configurators)
            {
                transcieverBuilder.Configure(configurator);
            }

            transcieverBuilder.Build();

            foreach (var configurator in configurators)
            {
                A.CallTo(() => configurator.Invoke(A<BuilderContext>._)).MustHaveHappened();
            }
        }
    }
}
