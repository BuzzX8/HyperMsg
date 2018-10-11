using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
	public class ObservableListenerTests
	{
		[Fact]
		public void OnNext_Redirects_Call_To_Observer()
		{
			var expected = Guid.NewGuid().ToString();
			var actual = (string) null;
			var observer = Observer.Create<string>(s => actual = s);
			var listener = new ObservableListenerImpl(observer);

			listener.OnNext(expected);

			Assert.Equal(expected, actual);
		}

		[Fact]
		public void OnCompleted_Redirects_Call_To_Observer()
		{
			var wasCalled = false;
			var observer = Observer.Create<string>(s => { }, () => wasCalled = true);
			var listener = new ObservableListenerImpl(observer);

			listener.OnCompleted();

			Assert.True(wasCalled);
		}

		[Fact]
		public void OnError_Redirects_Call_To_Observer()
		{
			var expected = new Exception();
			var actual = (Exception) null;
			var observer = Observer.Create<string>(s => { }, ex => actual = ex);
			var listener = new ObservableListenerImpl(observer);

			listener.OnError(expected);

			Assert.Same(expected, actual);
		}
	}

	internal class ObservableListenerImpl : ObservableListener<string>
	{
		public ObservableListenerImpl(IObserver<string> observer) : base(observer)
		{ }

		protected override Task DoListening(CancellationToken token) => Task.CompletedTask;

		public new void OnNext(string value) => base.OnNext(value);

		public new void OnCompleted() => base.OnCompleted();

		public new void OnError(Exception ex) => base.OnError(ex);
	}
}
