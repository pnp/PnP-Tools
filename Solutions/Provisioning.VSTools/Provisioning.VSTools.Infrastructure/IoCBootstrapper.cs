using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools
{
    public static class IoCBootstrapper
    {
        private static Container _container = null;

        public static Container GetContainer(bool registerGenericLogger = true)
        {
            if (_container == null)
            {
                _container = new Container();

                //register types
                if (registerGenericLogger)
                {
                    _container.Register<Services.ILogService, Services.GenericLogService>(SimpleInjector.Lifestyle.Singleton);
                }
                _container.Register<Services.IProvisioningService, Services.ProvisioningService>(SimpleInjector.Lifestyle.Transient);
            }

            return _container;
        }

        public static Services.ILogService GetLoggerInstance()
        {
            var c = GetContainer();
            
            return c.GetInstance<Services.ILogService>();
        }
    }
}
