using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HyperMsg
{
    public class ServiceProviderExtensionsTests
    {
        private readonly IServiceProvider serviceProvider = A.Fake<IServiceProvider>();

        [Fact]
        public void GetService_Returns_Service_Of_Specified_Type()
        {
            var service = A.Fake<IMessageSender>();
            A.CallTo(() => serviceProvider.GetService(typeof(IMessageSender))).Returns(service);

            var actual = serviceProvider.GetService<IMessageSender>();

            Assert.Same(service, actual);
        }

        [Fact]
        public void GetService_Throws_Exception_If_Incorrect_Service_Type_Provided()
        {
            var service = A.Fake<IMessageHandlerRegistry>();
            A.CallTo(() => serviceProvider.GetService(typeof(IMessageSender))).Returns(service);

            Assert.Throws<InvalidOperationException>(() => serviceProvider.GetService<IMessageSender>());
        }

        [Fact]
        public void GetService_Throws_Exception_()
        {
            A.CallTo(() => serviceProvider.GetService(A<Type>._)).Returns(null);

            Assert.Throws<InvalidOperationException>(() => serviceProvider.GetService<IMessageSender>(true));
        }
    }
}
