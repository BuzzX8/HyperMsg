using FakeItEasy;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BackgroundReceiverTests : IDisposable
    {
        private readonly IReceiver<Guid> messageReceiver;
        private readonly List<IHandler<Guid>> messageHandlers;
        private readonly BackgroundReceiver<Guid> backgroundReceiver;

        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        public BackgroundReceiverTests()
        {
            messageReceiver = A.Fake<IReceiver<Guid>>();
            messageHandlers = new List<IHandler<Guid>>();
            backgroundReceiver = new BackgroundReceiver<Guid>(messageReceiver, messageHandlers);            
        }

        [Fact]
        public void Run_Invokes_Message_Handler()
        {
            var expected = Guid.NewGuid();
            var actual = Guid.Empty;            
            var @event = new ManualResetEventSlim();
            A.CallTo(() => messageReceiver.ReceiveAsync(A<CancellationToken>._)).Returns(Task.FromResult(expected));
            var handler = A.Fake<IHandler<Guid>>();
            A.CallTo(() => handler.HandleAsync(expected, A<CancellationToken>._)).Invokes(foc =>
            {
                actual = foc.GetArgument<Guid>(0);
                @event.Set();
            });
            messageHandlers.Add(handler);

            Assert.False(backgroundReceiver.IsRunning);
            backgroundReceiver.Run();
            Assert.True(backgroundReceiver.IsRunning);
            @event.Wait(waitTimeout);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Dispose_Initiates_Background_Task_Cancellation()
        {
            var @event = new ManualResetEventSlim();
            var isTaskCompleted = false;

            backgroundReceiver.BackgroundTaskCompleted += t =>
            {
                isTaskCompleted = true;
                @event.Set();
            };
            backgroundReceiver.Run();
            
            backgroundReceiver.Dispose();
            @event.Wait(waitTimeout);

            Assert.True(isTaskCompleted);
        }

        [Fact]
        public void UnhandledException_Rises_With_Correct_Exception()
        {
            var @event = new ManualResetEventSlim();
            var expected = new InvalidOperationException();
            var actual = default(Exception);

            backgroundReceiver.UnhandledException += e =>
            {
                actual = e;
                @event.Set();
            };
            var handler = A.Fake<IHandler<Guid>>();
            A.CallTo(() => handler.HandleAsync(A<Guid>._, A<CancellationToken>._)).Returns(Task.FromException(expected));
            messageHandlers.Add(handler);

            backgroundReceiver.Run();
            @event.Wait(waitTimeout);
            
            Assert.Equal(expected, actual);
        }

        public void Dispose()
        {
            backgroundReceiver.Stop();
        }
    }
}
