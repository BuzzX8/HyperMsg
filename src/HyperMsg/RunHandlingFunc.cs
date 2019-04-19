using System;

namespace HyperMsg
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="handler"></param>
    /// <returns></returns>
    public delegate IDisposable RunHandlingFunc<T>(Action<T> handler);
}
