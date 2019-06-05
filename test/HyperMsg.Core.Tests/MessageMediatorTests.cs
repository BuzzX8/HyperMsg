﻿using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageMediatorTests
    {
        private readonly MessageMediator mediator = new MessageMediator();

        [Fact]
        public void Send_Calls_Handle_For_Each_Registered_Handler()
        {
            var handlers = A.CollectionOfFake<IHandler<string>>(4);
            AddHandlers(handlers);

            var expected = Guid.NewGuid().ToString();            

            mediator.Send(expected);

            foreach (var handler in handlers)
            {
                A.CallTo(() => handler.Handle(expected)).MustHaveHappened();
            }
        }

        [Fact]
        public async Task SendAsync_Calls_Handle_For_Each_Registered_Handler()
        {
            var handlers = A.CollectionOfFake<IHandler<string>>(4);
            AddHandlers(handlers);
            var expected = Guid.NewGuid().ToString();
            var cancellationToken = new CancellationToken();            

            await mediator.SendAsync(expected, cancellationToken);

            foreach (var handler in handlers)
            {
                A.CallTo(() => handler.HandleAsync(expected, cancellationToken)).MustHaveHappened();
            }
        }

        private void AddHandlers<T>(IEnumerable<IHandler<T>> handlers)
        {
            foreach (var handler in handlers)
            {
                mediator.Register(handler);
            }
        }
    }
}
