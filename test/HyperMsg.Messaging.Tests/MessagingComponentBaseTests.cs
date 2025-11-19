using FakeItEasy;
using Xunit;

namespace HyperMsg.Messaging;

public class MessagingComponentBaseTests
{
    [Fact]
    public void Attach_Calls_RegisterHandlers_With_Context()
    {
        var context = A.Fake<IMessagingContext>();
        var component = new TestComponent([]);

        component.Attach(context);

        Assert.Same(context, component.CapturedContext);
    }

    [Fact]
    public void Dispose_Disposes_Returned_Disposables()
    {
        var disposable1 = A.Fake<IDisposable>();
        var disposable2 = A.Fake<IDisposable>();
        var component = new TestComponent([disposable1, disposable2]);

        component.Attach(A.Fake<IMessagingContext>());

        component.Dispose();

        A.CallTo(() => disposable1.Dispose()).MustHaveHappenedOnceExactly();
        A.CallTo(() => disposable2.Dispose()).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void Detach_Invokes_Dispose_On_Disposables()
    {
        var disposable = A.Fake<IDisposable>();
        var component = new TestComponent([disposable]);

        component.Attach(A.Fake<IMessagingContext>());

        component.Detach(A.Fake<IMessagingContext>());

        A.CallTo(() => disposable.Dispose()).MustHaveHappenedOnceExactly();
    }

    private sealed class TestComponent(IEnumerable<IDisposable> handlers) : MessagingComponentBase
    {
        private readonly IEnumerable<IDisposable> handlers = handlers;

        public IMessagingContext? CapturedContext { get; private set; }

        protected override IEnumerable<IDisposable> RegisterHandlers(IMessagingContext messagingContext)
        {
            CapturedContext = messagingContext;
            return handlers;
        }
    }
}
