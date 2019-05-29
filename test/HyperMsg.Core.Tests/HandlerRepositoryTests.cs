using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace HyperMsg
{
    public class HandlerRepositoryTests
    {
        [Fact]
        public void GetHandlers_Returns_Null_If_No_Handlers_Registred_For_Specified_Type()
        {
            var repository = new HandlerRepository();
            repository.AddHandler(A.Fake<IHandler<Guid>>());

            var handlers = repository.GetHandlers<string>();

            Assert.Null(handlers);
        }

        [Fact]
        public void GetHandlers_Returns_Previously_Added_Handlers()
        {
            var repository = new HandlerRepository();
            var handlers = A.CollectionOfFake<IHandler<string>>(4);

            foreach (var handler in handlers)
            {
                repository.AddHandler(handler);
            }

            var actual = repository.GetHandlers<string>();

            Assert.Equal(handlers, actual);
        }
    }
}