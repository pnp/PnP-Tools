using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace SharePoint.Modernization.Framework.Functions
{
    public abstract class FunctionsBase
    {
        protected ClientContext clientContext;

        public FunctionsBase(ClientContext clientContext)
        {
            this.clientContext = clientContext;
        }
    }
}
