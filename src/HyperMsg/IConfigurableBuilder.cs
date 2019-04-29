using System;

namespace HyperMsg
{
    public interface IConfigurableBuilder<T>
    {
        void Configure(Action<BuilderContext> configurator);

        T Build();
    }
}