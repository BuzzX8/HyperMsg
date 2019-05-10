using System;

namespace HyperMsg
{
    public interface IConfigurable
    {
        void Configure(Action<Configuration> configurator);

        void Configure(Action<Configuration> configurator, string settingsName, object settings);
    }
}