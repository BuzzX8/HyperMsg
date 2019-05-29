using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public interface IConfigurable
    {
        void AddSetting(string settingName, object setting);

        void RegisterConfigurator(Configurator configurator);

        void RegisterService(Type serviceInterface, ServiceFactory serviceFactory);

        void RegisterService(IEnumerable<Type> serviceInterfaces, ServiceFactory serviceFactory);
    }
}