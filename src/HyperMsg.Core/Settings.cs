using System;

namespace HyperMsg
{
    internal class Settings : ISettings
    {
        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void Set<T>(string key, T value)
        {
            throw new NotImplementedException();
        }
    }
}
