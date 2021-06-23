using FakeItEasy;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageBrokerExtensionTests
    {
        private readonly MessageBroker broker = new();

        [Fact]
        public void RegisterHandler_Invokes_Handler_If_Predicate_Returns_True()
        {            
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterHandler(_ => true, handler);

            broker.Send(Guid.NewGuid());

            A.CallTo(() => handler.Invoke(A<Guid>._)).MustHaveHappened();
        }

        [Fact]
        public void RegisterHandler_Does_Not_Invokes_Handler_If_Predicate_Returns_False()
        {
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterHandler(_ => false, handler);

            broker.Send(Guid.NewGuid());

            A.CallTo(() => handler.Invoke(A<Guid>._)).MustNotHaveHappened();
        }

        private readonly Guid pipeId = Guid.NewGuid();
        private readonly Guid portId = Guid.NewGuid();
        private readonly Guid message = Guid.NewGuid();

        [Fact]
        public void SendToPipe_Invokes_Handler_Registered_With_RegisterPipeHandler()
        {
            var filter = A.Fake<Action<Guid>>();
            broker.RegisterPipeHandler(pipeId, portId, filter);

            broker.SendToPipe(pipeId, portId, message);

            A.CallTo(() => filter.Invoke(message)).MustHaveHappened();
        }

        [Fact]
        public async Task SendToPipeAsync_Invokes_Handler_Registered_With_RegisterPipeHandler()
        {
            var filter = A.Fake<AsyncAction<Guid>>();
            broker.RegisterPipeHandler(pipeId, portId, filter);

            await broker.SendToPipeAsync(pipeId, portId, message);

            A.CallTo(() => filter.Invoke(message, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void SendToPipe_Does_Not_Invokes_Pipe_Handler_With_Different_PortId()
        {
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterPipeHandler(pipeId, portId, handler);
            broker.SendToPipe(pipeId, Guid.NewGuid(), message);
            A.CallTo(() => handler.Invoke(message)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SendToPipeAsync_Does_Not_Invokes_Pipe_Handler_With_Different_PortId()
        {
            var handler = A.Fake<AsyncAction<Guid>>();
            broker.RegisterPipeHandler(pipeId, portId, handler);

            await broker.SendToPipeAsync(pipeId, Guid.NewGuid(), message);

            A.CallTo(() => handler.Invoke(message, A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Fact]
        public void SendToPipe_Does_Not_Invokes_Pipe_Handler_With_Different_PipeId()
        {
            var handler = A.Fake<Action<Guid>>();
            broker.RegisterPipeHandler(pipeId, portId, handler);

            broker.SendToPipe(Guid.NewGuid(), portId, message);

            A.CallTo(() => handler.Invoke(message)).MustNotHaveHappened();
        }

        [Fact]
        public async Task SendToPipeAsync_Does_Not_Invokes_Pipe_Handler_With_Different_PipeId()
        {
            var handler = A.Fake<AsyncAction<Guid>>();
            broker.RegisterPipeHandler(pipeId, portId, handler);

            await broker.SendToPipeAsync(Guid.NewGuid(), portId, message);

            A.CallTo(() => handler.Invoke(message, A<CancellationToken>._)).MustNotHaveHappened();
        }
    }
}
