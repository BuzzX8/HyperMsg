using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperMsg
{
    public class HandlerRepository : IHandlerRepository
    {
        private readonly Dictionary<Type, List<object>> handlers = new Dictionary<Type, List<object>>();

        public void AddHandler<T>(IHandler<T> handler)
        {
            if (!handlers.ContainsKey(typeof(T)))
            {
                handlers.Add(typeof(T), new List<object>());
            }

            handlers[typeof(T)].Add(handler);
        }

        public IEnumerable<IHandler<T>> GetHandlers<T>()
        {
            if (handlers.ContainsKey(typeof(T)))
            {
                return handlers[typeof(T)].Cast<IHandler<T>>();
            }

            return null;
        }
    }
}
