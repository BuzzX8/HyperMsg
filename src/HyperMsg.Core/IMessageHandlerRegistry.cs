using System;
using System.Threading.Tasks;

namespace HyperMsg
{
    interface IMessageHandlerRegistry
    {
	    void Register<T>(Action<T> handler);

	    void Register<T>(Action<T> handler, Func<T, bool> filter);

	    void Register<T>(Func<T, Task> handler);

	    void Register<T>(Func<T, Task> handler, Func<T, bool> filter);
    }
}
