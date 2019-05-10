using System;

namespace HyperMsg
{
    public interface IConfigurable
    {
        void AddSetting(string settingName, object setting);

        void Configure(Action<Configuration> configurator);
    }
}