using System;
using System.Collections.Generic;

namespace HyperMsg
{
    public interface IConfigurable
    {
        void AddSetting(string settingName, object setting);

        void AddService(Type serviceInterface, ServiceFactory serviceFactory);

        void AddService(IEnumerable<Type> serviceInterfaces, ServiceFactory serviceFactory);
    }
}