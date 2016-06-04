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
        public static Container GetContainer(bool registerGenericLogger = true)
        {
            Container c = new Container();

            //register types
            if (registerGenericLogger)
            {
                c.Register<Services.ILogService, Services.GenericLogService>(SimpleInjector.Lifestyle.Singleton);
            }
            c.Register<Services.IProvisioningService, Services.ProvisioningService>(SimpleInjector.Lifestyle.Transient);

            return c;
        }
    }
}
