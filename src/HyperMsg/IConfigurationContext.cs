using System;

namespace HyperMsg
{
    public interface IConfigurationContext : IServiceProvider
    {
        object GetSetting(string settingName);

        void RegisterService(Type serviceType, object serviceInstance);
    }
}