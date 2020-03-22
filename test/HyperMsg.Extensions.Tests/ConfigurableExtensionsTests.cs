using FakeItEasy;
using Xunit;

namespace HyperMsg
{
    public class ConfigurableExtensionsTests
    {
        private readonly IConfigurable configurable = A.Fake<IConfigurable>();

        [Fact]
        public void AddService_Registers_Service_Instance()
        {
            var actual = default(object);
            var expected = A.Fake<IMessageSender>();
            A.CallTo(() => configurable.AddService(typeof(IMessageSender), A<object>._)).Invokes(foc =>
            {
                actual = foc.GetArgument<object>(1);
            });

            configurable.AddService(expected);

            Assert.Same(expected, actual);
        }
    }
}