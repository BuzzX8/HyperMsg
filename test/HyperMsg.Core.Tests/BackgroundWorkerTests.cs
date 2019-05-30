using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BackgroundWorkerTests : IDisposable
    {
        private BackgroundWorkerImpl worker;
        private readonly TimeSpan waitTimeOut = TimeSpan.FromSeconds(2);

        [Fact]
        public void Run_Periodically_Invokes_DoWorkIterationAsync()
        {
            var invokeCount = 5;
            var @event = new ManualResetEventSlim();
            worker = new BackgroundWorkerImpl(t => 
            {
                if (invokeCount == 0)
                {
                    @event.Set();
                    return Task.CompletedTask;
                }

                invokeCount--;
                return Task.CompletedTask;
            });

            worker.InvokeRun();
            @event.Wait(waitTimeOut);

            Assert.Equal(0, invokeCount);
        }

        [Fact]
        public void Stop_Stops_Invoking_DoWorkIterationAsync_And_Invokes_BackgroundTaskCompleted()
        {
            var @event = new ManualResetEventSlim();
            var @event2 = new ManualResetEventSlim();
            var wasInvoked = false;
            worker = new BackgroundWorkerImpl(t =>
            {
                @event.Wait();
                wasInvoked = true;
                return Task.CompletedTask;
            });
            worker.BackgroundTaskCompleted += t => event2.Set();
            worker.InvokeRun();

            worker.InvokeStop();
            @event.Set();
            event2.Wait(waitTimeOut);

            Assert.False(wasInvoked);
        }

        [Fact]
        public void Invokes_UnhandledException_When_DoWorkIterationAsync_Throws_Exception()
        {
            var @event = new ManualResetEventSlim();
            var expected = new InvalidOperationException();
            var actual = default(Exception);
            worker = new BackgroundWorkerImpl(t => throw expected);
            worker.UnhandledException += e =>
            {
                actual = e;
                @event.Set();
            };

            worker.InvokeRun();
            @event.Wait(waitTimeOut);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Dispose_Initiates_Background_Task_Cancellation()
        {
            var @event = new ManualResetEventSlim();
            var isTaskCompleted = false;
            worker = new BackgroundWorkerImpl(t => Task.Delay(waitTimeOut));
            worker.BackgroundTaskCompleted += t =>
            {
                isTaskCompleted = true;
                @event.Set();
            };
            worker.InvokeRun();

            worker.Dispose();
            @event.Wait(waitTimeOut);

            Assert.True(isTaskCompleted);
        }

        public void Dispose() => worker?.Dispose();
    }

    public class BackgroundWorkerImpl : BackgroundWorker
    {
        private readonly Func<CancellationToken, Task> workIterationFunc;

        public BackgroundWorkerImpl(Func<CancellationToken, Task> workIterationFunc)
        {
            this.workIterationFunc = workIterationFunc;
        }

        protected override Task DoWorkIterationAsync(CancellationToken cancellationToken) => workIterationFunc(cancellationToken);

        public void InvokeRun() => Run();

        public void InvokeStop() => Stop();
    }
}
