using System;

namespace HyperMsg.Transciever
{
    public interface ISubject<T> : IObserver<T>, IObservable<T>
    { }
}
