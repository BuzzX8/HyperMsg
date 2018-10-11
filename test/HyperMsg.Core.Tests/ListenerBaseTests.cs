using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
	public class ListenerBaseTests
	{
		private readonly TimeSpan waitTimeout = TimeSpan.FromSeconds(2);

		[Fact]
		public void Start_Runs_Listener_Task()
		{
			var @event = new ManualResetEventSlim();
			var wasCalled = false;
			var listener = new ListenerImpl(t =>
			{
				wasCalled = true;
				@event.Set();
				return Task.CompletedTask;
			});

			listener.Start();
			@event.Wait(waitTimeout);
			
			Assert.True(wasCalled);
		}

		[Fact]
		public void Start_Rises_Started_Event()
		{
			var wasCalled = false;
			var listener = new ListenerImpl(t => Task.CompletedTask);
			listener.Started += (s, e) => wasCalled = true;

			listener.Start();

			Assert.True(wasCalled);
		}

		[Fact]
		public void Rises_Completed_When_Task_Completed()
		{
			var wasRaised = false;
			var listener = new ListenerImpl(t => Task.CompletedTask);
			var @event = new ManualResetEventSlim();
			listener.Completed += (s, e) =>
			{
				wasRaised = true;
				@event.Set();
			};
			
			listener.Start();
			@event.Wait(waitTimeout);

			Assert.True(wasRaised);
		}

		[Fact]
		public void Rises_Error_If_Task_Throws_Exception()
		{
			var wasRaised = false;
			var listener = new ListenerImpl(t => throw new Exception());
			var @event = new ManualResetEventSlim();
			listener.Error += (s, e) =>
			{
				wasRaised = true;
				@event.Set();
			};
			
			listener.Start();
			@event.Wait(waitTimeout);

			Assert.True(wasRaised);
		}

		[Fact]
		public void Stop_Rises_Stopped_Event()
		{
			var wasRaised = false;
			var listener = new ListenerImpl(t => Task.CompletedTask);
			var @event = new ManualResetEventSlim();
			listener.Stopped += (s, e) =>
			{
				wasRaised = true;
				@event.Set();
			};
			listener.Start();

			listener.Stop();
			@event.Wait(waitTimeout);

			Assert.True(wasRaised);
		}
	}

	internal class ListenerImpl : ListenerBase
	{
		private readonly Func<CancellationToken, Task> doListening;

		public ListenerImpl(Func<CancellationToken, Task> doListening)
		{
			this.doListening = doListening;
		}

		protected override Task DoListening(CancellationToken token)
		{
			return doListening(token);
		}
	}
}
