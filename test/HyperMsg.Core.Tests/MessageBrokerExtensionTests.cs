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

        [Fact]
        public void SendSerializeCommand_Invokes_Handler_Registtered_By_RegisterSerializationHandler()
        {
            var handler = A.Fake<Action<IBufferWriter, Guid>>();
            var bufferWriter = A.Fake<IBufferWriter>();
            var message = Guid.NewGuid();

            broker.RegisterSerializationHandler(handler);
            broker.SendSerializeCommand(bufferWriter, message);

            A.CallTo(() => handler.Invoke(bufferWriter, message)).MustHaveHappened();
        }

        [Fact]
        public void SendRequest_Invokes_Request_Handler()
        {
            var handler = A.Fake<Func<string, int>>();
            var request = Guid.NewGuid().ToString();
            broker.RegisterRequestHandler(handler);

            broker.SendRequest<string, int>(request);

            A.CallTo(() => handler.Invoke(request)).MustHaveHappened();
        }

        [Fact]
        public async Task SendRequestAsync_Invokes_Request_Handler()
        {
            var handler = A.Fake<Func<string, CancellationToken, Task<int>>>();
            var request = Guid.NewGuid().ToString();
            broker.RegisterRequestHandler(handler);

            await broker.SendRequestAsync<string, int>(request);

            A.CallTo(() => handler.Invoke(request, A<CancellationToken>._)).MustHaveHappened();
        }

        [Fact]
        public void SendRequest_Returns_Response_From_Handler()
        {
            var handler = A.Fake<Func<string, Guid>>();
            var request = Guid.NewGuid().ToString();
            var response = Guid.NewGuid();
            broker.RegisterRequestHandler(handler);
            A.CallTo(() => handler.Invoke(request)).Returns(response);

            var actualResponse = broker.SendRequest<string, Guid>(request);

            Assert.Equal(response, actualResponse);
        }

        [Fact]
        public async Task SendRequestAsync_Returns_Response_From_Handler()
        {
            var handler = A.Fake<Func<string, CancellationToken, Task<Guid>>>();
            var request = Guid.NewGuid().ToString();
            var response = Guid.NewGuid();
            broker.RegisterRequestHandler(handler);
            A.CallTo(() => handler.Invoke(request, A<CancellationToken>._)).Returns(Task.FromResult(response));

            var actualResponse = await broker.SendRequestAsync<string, Guid>(request);

            Assert.Equal(response, actualResponse);
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

        [Fact]
        public void SendToPipe_Invokes_Port_Filter()
        {
            var handler = A.Fake<Action<Guid>>();
            var filter = A.Fake<Func<object, bool>>();
            broker.RegisterPipeFilter(pipeId, filter, handler);

            broker.SendToPipe(pipeId, portId, message);

            A.CallTo(() => filter.Invoke(portId)).MustHaveHappened();
        }

        [Fact]
        public void SendToPipe_Invokes_Async_Port_Filter()
        {
            var handler = A.Fake<AsyncAction<Guid>>();
            var filter = A.Fake<Func<object, bool>>();
            broker.RegisterPipeFilter(pipeId, filter, handler);

            broker.SendToPipe(pipeId, portId, message);

            A.CallTo(() => filter.Invoke(portId)).MustHaveHappened();
        }
    }
}
