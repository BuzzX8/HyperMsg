using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class BackgroundWorkerTests : IDisposable
    {
        private BackgroundWorker worker;
        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        [Fact]
        public async Task Run_Periodically_Invokes_DoWorkIterationAsync()
        {
            var invokeCount = 5;
            var @event = new ManualResetEventSlim();
            worker = new BackgroundWorker(async t => 
            {
                if (invokeCount == 0)
                {
                    await StopWorkerAsync(t);
                    @event.Set();
                }

                invokeCount--;
            });

            await RunWorkerAsync();
            @event.Wait(waitTimeout);

            Assert.Equal(0, invokeCount);
        }

        [Fact]
        public async Task Stop_Stops_Invoking_DoWorkIterationAsync_And_Invokes_BackgroundTaskCompleted()
        {
            var @event = new ManualResetEventSlim();
            var @event2 = new ManualResetEventSlim();
            var wasInvoked = false;
            worker = new BackgroundWorker(t =>
            {
                @event.Wait();
                wasInvoked = true;
                return Task.CompletedTask;
            });
            worker.BackgroundTaskCompleted += t => event2.Set();
            await RunWorkerAsync();

            await StopWorkerAsync();
            @event.Set();
            event2.Wait(waitTimeout);

            Assert.False(wasInvoked);
        }

        [Fact]
        public async Task Invokes_UnhandledException_When_DoWorkIterationAsync_Throws_Exception()
        {
            var @event = new ManualResetEventSlim();
            var expected = new InvalidOperationException();
            var actual = default(Exception);
            worker = new BackgroundWorker(t => throw expected);
            worker.UnhandledException += e =>
            {
                actual = e;
                @event.Set();
            };

            await RunWorkerAsync();
            @event.Wait(waitTimeout);

            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task Dispose_Initiates_Background_Task_Cancellation()
        {
            var @event = new ManualResetEventSlim();
            var isTaskCompleted = false;
            worker = new BackgroundWorker(t => Task.Delay(waitTimeout));
            worker.BackgroundTaskCompleted += t =>
            {
                isTaskCompleted = true;
                @event.Set();
            };
            await RunWorkerAsync();

            worker.Dispose();
            @event.Wait(waitTimeout);

            Assert.True(isTaskCompleted);
        }

        [Fact]
        public async Task Stop_Completes_BackgroundTask_When_DoWorkIterationAsync_Locked()
        {
            var @event = new ManualResetEventSlim();
            var event2 = new ManualResetEventSlim();
            worker = new BackgroundWorker(t =>
            {
                @event.Wait();
                return Task.CompletedTask;
            });
            worker.BackgroundTaskCompleted += t => event2.Set();
            await RunWorkerAsync();

            await StopWorkerAsync();
            event2.Wait(waitTimeout);

            Assert.True(event2.IsSet);
        }

        private Task RunWorkerAsync(CancellationToken cancellationToken = default) => worker.HandleTransportEventAsync(new TransportEventArgs(TransportEvent.Opened), CancellationToken.None);

        private Task StopWorkerAsync(CancellationToken cancellationToken = default) => worker.HandleTransportEventAsync(new TransportEventArgs(TransportEvent.Closing), CancellationToken.None);

        public void Dispose() => worker?.Dispose();
    }
}