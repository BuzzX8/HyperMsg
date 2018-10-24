using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
	public class BackgroundWorkerTests
	{
		private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

		[Fact]
		public void Start_Runs_Listener_Task()
		{
			var @event = new ManualResetEventSlim();
			var wasCalled = false;
			var listener = new BackgroundWorkerImpl(t =>
			{
				wasCalled = true;
				@event.Set();
				return Task.CompletedTask;
			});

            using (listener.Run())
            {
                @event.Wait(waitTimeout);

                Assert.True(wasCalled);
            }
		}

		[Fact]
		public void Start_Rises_Started_Event()
		{
			var wasCalled = false;
			var listener = new BackgroundWorkerImpl(t => Task.CompletedTask);
			listener.Started += (s, e) => wasCalled = true;

            using (listener.Run())
                Assert.True(wasCalled);
		}

		[Fact]
		public void Rises_Completed_When_Task_Completed()
		{
			var wasRaised = false;
			var listener = new BackgroundWorkerImpl(t => Task.CompletedTask);
			var @event = new ManualResetEventSlim();
			listener.Completed += (s, e) =>
			{
				wasRaised = true;
				@event.Set();
			};

            using (listener.Run())
            {
                @event.Wait(waitTimeout);

                Assert.True(wasRaised);
            }
		}

		[Fact]
		public void Rises_Error_If_Task_Throws_Exception()
		{
			var wasRaised = false;
			var listener = new BackgroundWorkerImpl(t => throw new Exception());
			var @event = new ManualResetEventSlim();
			listener.Error += (s, e) =>
			{
				wasRaised = true;
				@event.Set();
			};

            using (listener.Run())
            {
                @event.Wait(waitTimeout);

                Assert.True(wasRaised);
            }
		}

		[Fact]
		public void Stop_Rises_Stopped_Event()
		{
			var wasRaised = false;
			var listener = new BackgroundWorkerImpl(t => Task.CompletedTask);
			var @event = new ManualResetEventSlim();
			listener.Stopped += (s, e) =>
			{
				wasRaised = true;
				@event.Set();
			};
			var disposable = listener.Run();

            disposable.Dispose();
			@event.Wait(waitTimeout);

			Assert.True(wasRaised);
		}
	}

	internal class BackgroundWorkerImpl : BackgroundWorker
	{
		private readonly Func<CancellationToken, Task> doListening;

		public BackgroundWorkerImpl(Func<CancellationToken, Task> doListening)
		{
			this.doListening = doListening;
		}

		protected override Task DoWorkAsync(CancellationToken token)
		{
			return doListening(token);
		}
	}
}
