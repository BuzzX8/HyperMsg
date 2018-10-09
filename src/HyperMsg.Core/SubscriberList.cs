using System;
using System.Collections.Generic;
using System.Text;

namespace HyperMsg
{
    internal class SubscriberList<T>
    {
        public IEnumerable<IObserver<T>> Observers { get; }

        public IDisposable Add(IObserver<T> observer)
        {
            return null;
        }
    }
}
