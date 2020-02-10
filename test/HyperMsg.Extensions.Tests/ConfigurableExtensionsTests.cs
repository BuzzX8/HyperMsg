using FakeItEasy;
using Xunit;

namespace HyperMsg
{
    public class ConfigurableExtensionsTests
    {
        private readonly IConfigurable configurable = A.Fake<IConfigurable>();

        [Fact]
        public void RegisterService_Registers_Service_Instance()
        {
            var serviceFactory = default(ServiceFactory);
            var serviceInstance = A.Fake<IMessageSender>();
            A.CallTo(() => configurable.RegisterService(typeof(IMessageSender), A<ServiceFactory>._)).Invokes(foc =>
            {
                serviceFactory = foc.GetArgument<ServiceFactory>(1);
            });

            configurable.RegisterService(serviceInstance);

            Assert.NotNull(serviceFactory);
        }
    }
}