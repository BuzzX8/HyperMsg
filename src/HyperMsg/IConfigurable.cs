using System;

namespace HyperMsg
{
    public interface IConfigurable
    {
        void Configure(Action<BuilderContext> configurator);
    }
}