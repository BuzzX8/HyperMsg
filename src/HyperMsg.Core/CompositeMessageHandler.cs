using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperMsg
{
    public class CompositeMessageHandler : IMessageHandlerRegistry
    {
		private readonly List<object> handlers = new List<object>();
		private readonly List<object> asyncHandlers = new List<object>();
	    
	    public void Handle<T>(T message)
	    {
		    foreach (var handler in handlers.OfType<Action<T>>())
		    {
			    handler(message);
		    }

		    var tasks = asyncHandlers
			    .OfType<Func<T, Task>>()
			    .Select(h => h(message))
			    .ToArray();

		    Task.WaitAll(tasks);
	    }
		
	    public void Register<T>(Action<T> handler)
	    {
		    handlers.Add(handler);
	    }

	    public void Register<T>(Action<T> handler, Func<T, bool> filter)
	    {
		    Register((Action<T>)new ConditionalMessageHandler<T>(handler, filter).Handle);
	    }

	    public void Register<T>(Func<T, Task> handler)
	    {
		    asyncHandlers.Add(handler);
	    }

	    public void Register<T>(Func<T, Task> handler, Func<T, bool> filter)
	    {
		    Register((Func<T, Task>)new ConditionalMessageHandler<T>(handler, filter).HandleAsync);
	    }
    }
}
