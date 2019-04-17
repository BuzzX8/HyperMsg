using FakeItEasy;
using System;
using System.Buffers;
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

        [Fact]
        public void Build_Returns_Transciever_Which_Can_Send_Messages()
        {
            var transciever = transcieverBuilder.Build();
            Assert.NotNull(transciever);

            var message = Guid.NewGuid();
            transciever.Send(message);

            A.CallTo(() => serializer.Serialize(A<IBufferWriter<byte>>._, message)).MustHaveHappened();
        }

        [Fact]
        public void Build_Returns_Transciever_Which_Can_Receive_Messages()
        {
            var expectedMessage = Guid.NewGuid();
            A.CallTo(() => serializer.Deserialize(A<ReadOnlySequence<byte>>._)).Returns(new DeserializationResult<Guid>(1, expectedMessage));
            var transciever = transcieverBuilder.Build();
            Assert.NotNull(transciever);

            var actualMessage = transciever.Receive();

            Assert.Equal(expectedMessage, actualMessage);
            A.CallTo(() => serializer.Deserialize(A<ReadOnlySequence<byte>>._)).MustHaveHappened();
        }
    }
}
