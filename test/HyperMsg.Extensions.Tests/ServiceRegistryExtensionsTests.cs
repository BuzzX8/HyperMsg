using FakeItEasy;
using Xunit;

namespace HyperMsg
{
    public class ServiceRegistryExtensionsTests
    {
        private readonly IServiceRegistry serviceRegistry = A.Fake<IServiceRegistry>();

        [Fact]
        public void AddService_Registers_Service_Instance()
        {
            var actual = default(object);
            var expected = A.Fake<ISender>();
            A.CallTo(() => serviceRegistry.Add(typeof(ISender), A<object>._)).Invokes(foc =>
            {
                actual = foc.GetArgument<object>(1);
            });

            serviceRegistry.AddService(expected);

            Assert.Same(expected, actual);
        }
    }
}