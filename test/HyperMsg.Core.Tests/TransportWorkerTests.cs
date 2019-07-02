using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class TransportWorkerTests : IDisposable
    {
        private TransportWorker worker;
        private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

        [Fact]
        public async Task OpenTransport_Periodically_Invokes_AsyncAction()
        {
            var invokeCount = 5;
            var @event = new ManualResetEventSlim();
            worker = new TransportWorker(async t => 
            {
                if (invokeCount == 0)
                {
                    await StopWorkerAsync(t);
                    @event.Set();
                    return;
                }

                invokeCount--;
            });

            await RunWorkerAsync();
            @event.Wait(waitTimeout);

            Assert.Equal(0, invokeCount);
        }

        [Fact]
        public async Task ClosedTransport_Stops_Invoking_AsyncAction_And_Invokes_BackgroundTaskCompleted()
        {
            var @event = new ManualResetEventSlim();
            var @event2 = new ManualResetEventSlim();
            var wasInvoked = false;
            worker = new TransportWorker(t =>
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
        public async Task Invokes_UnhandledException_When_AsyncAction_Throws_Exception()
        {
            var @event = new ManualResetEventSlim();
            var expected = new InvalidOperationException();
            var actual = default(Exception);
            worker = new TransportWorker(t => throw expected);
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
            worker = new TransportWorker(t => Task.Delay(waitTimeout));
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
        public async Task ClosedTransport_Completes_BackgroundTask_When_AsyncAction_Locked()
        {
            var @event = new ManualResetEventSlim();
            var event2 = new ManualResetEventSlim();
            worker = new TransportWorker(t =>
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

        private Task StopWorkerAsync(CancellationToken cancellationToken = default) => worker.HandleTransportEventAsync(new TransportEventArgs(TransportEvent.Closed), CancellationToken.None);

        public void Dispose() => worker?.Dispose();
    }
}