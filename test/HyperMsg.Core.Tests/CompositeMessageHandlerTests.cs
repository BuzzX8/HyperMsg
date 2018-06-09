using System;
using System.Threading.Tasks;
using Xunit;

namespace HyperMsg
{
    public class CompositeMessageHandlerTests
    {
	    private CompositeMessageHandler handler = new CompositeMessageHandler();

		[Fact]
	    public void Handle_Should_Invoke_Registered_Handler_And_Provide_Message()
		{
			bool isMsgCorrect = false;
			var message = Guid.NewGuid().ToString();
			handler.Register<string>(m => isMsgCorrect = m == message);

			handler.Handle(message);

			Assert.True(isMsgCorrect);
		}

	    [Fact]
	    public void Handle_Should_Invoke_Registered_Async_Handler_And_Provide_Message()
	    {
		    bool isMsgCorrect = false;
		    var message = Guid.NewGuid().ToString();
		    handler.Register<string>(m => Task.FromResult(isMsgCorrect = m == message));

		    handler.Handle(message);

		    Assert.True(isMsgCorrect);
		}

		[Fact]
	    public void Handle_Should_Not_Invoke_Handler_If_Filter_Returns_False()
	    {
		    bool wasInvoked = false;
			handler.Register<string>(m => wasInvoked = true, m => false);

			handler.Handle(Guid.NewGuid().ToString());

			Assert.False(wasInvoked);
	    }

	    [Fact]
	    public void Handle_Should_Not_Invoke_Async_Handler_If_Filter_Returns_False()
	    {
		    bool wasInvoked = false;
		    handler.Register<string>(m => Task.FromResult(wasInvoked = true), m => false);

		    handler.Handle(Guid.NewGuid().ToString());

		    Assert.False(wasInvoked);
	    }

		[Fact]
	    public void Handle_Should_Invoke_Handler_If_Filter_Returns_True()
	    {
		    bool wasInvoked = true;
		    handler.Register<string>(m => wasInvoked = true, m => true);

		    handler.Handle(Guid.NewGuid().ToString());

		    Assert.True(wasInvoked);
		}

		[Fact]
	    public void Handle_Should_Invoke_Async_Handler_If_Filter_Returns_True()
	    {
		    bool wasInvoked = true;
		    handler.Register<string>(m => Task.FromResult(wasInvoked = true), m => true);

		    handler.Handle(Guid.NewGuid().ToString());

		    Assert.True(wasInvoked);
		}
    }
}
