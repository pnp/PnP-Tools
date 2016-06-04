using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Provisioning.VSTools.Test
{
    [TestClass]
    public abstract class BaseTest
    {
        internal SimpleInjector.Container Container = null;
        public BaseTest()
        {
            this.Container = IoCBootstrapper.GetContainer();
        }
    }
}
