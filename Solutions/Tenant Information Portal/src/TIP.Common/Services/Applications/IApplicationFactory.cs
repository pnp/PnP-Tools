using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Online.Applications.Core;

namespace TIP.Common.Services.Applications
{
    public interface IApplicationFactory
    {
        IApplicationManager CreateInstance(IClient client);
    }
}
