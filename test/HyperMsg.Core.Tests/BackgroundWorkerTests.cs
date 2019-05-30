using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BackgroundWorkerTests
    {
        [Fact]
        public void Run_Periodically_Invokes_DoWorkIterationAsync()
        {
            var invokeCount = 5;
            var @event = new ManualResetEventSlim();
            var worker = new BackgroundWorkerImpl(t => 
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
            @event.Wait(TimeSpan.FromSeconds(2));

            Assert.Equal(0, invokeCount);
        }

        [Fact]
        public void Stop_Stops_Invoking_DoWorkIterationAsync()
        {
            var @event = new ManualResetEventSlim();
            var @event2 = new ManualResetEventSlim();
            var wasInvoked = false;
            var worker = new BackgroundWorkerImpl(t =>
            {
                @event.Wait();
                wasInvoked = true;
                return Task.CompletedTask;
            });
            worker.BackgroundTaskCompleted += t => event2.Set();
            worker.InvokeRun();

            worker.InvokeStop();
            @event.Set();
            event2.Wait(TimeSpan.FromSeconds(2));

            Assert.False(wasInvoked);
        }
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
