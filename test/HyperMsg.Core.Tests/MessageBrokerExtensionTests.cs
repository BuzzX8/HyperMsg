using FakeItEasy;
using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class MessageBrokerExtensionTests
    {
        private readonly MessageBroker broker = new();
        private readonly Guid data = Guid.NewGuid();
        private readonly Action<Guid> handler = A.Fake<Action<Guid>>();
        private readonly AsyncAction<Guid> asyncHandler = A.Fake<AsyncAction<Guid>>();

        [Fact]
        public void SendReceiveEvent_Sends_Message_To_Transmit_Handlers()
        {
            broker.RegisterReceiveEventHandler(handler);
            broker.RegisterReceiveEventHandler(asyncHandler);

            broker.SendReceiveEvent(data);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }

        [Fact]
        public async Task SendReceiveEventAsync_Sends_Message_To_Transmit_Handlers()
        {
            broker.RegisterReceiveEventHandler(handler);
            broker.RegisterReceiveEventHandler(asyncHandler);

            await broker.SendReceiveEventAsync(data, default);

            A.CallTo(() => handler.Invoke(data)).MustHaveHappened();
            A.CallTo(() => asyncHandler.Invoke(data, default)).MustHaveHappened();
        }

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
            var handler = A.Fake<Action<IBufferWriter<byte>, Guid>>();
            var bufferWriter = A.Fake<IBufferWriter<byte>>();
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

        [Fact]
        public void WaitMessage_Result()
        {
            var message = Guid.NewGuid();

            var task = broker.WaitMessage<Guid>(m => m == message, default);
            broker.Send(message);
                        
            Assert.Equal(message, task.Result);
        }

        [Fact]
        public void WaitMessage_Exception()
        {
            var message = Guid.NewGuid();
            var exception = new InvalidCastException();

            var task = broker.WaitMessage<Guid>(m => throw exception, default);
            broker.Send(Guid.NewGuid());

            var _ = Assert.Throws<AggregateException>(() => task.Wait(1000));
        }

        [Fact]
        public void WaitMessage_Cancel()
        {
            var cancellation = new CancellationTokenSource();

            var task = broker.WaitMessage<Guid>(m => false, cancellation.Token);
            broker.Send(Guid.NewGuid());

            cancellation.Cancel();

            Assert.True(task.IsCanceled);
        }

        [Fact]
        public void SendAndWaitMessage_()
        {

        }
    }
}
