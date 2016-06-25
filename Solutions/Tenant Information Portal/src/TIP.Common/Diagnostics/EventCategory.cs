using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIP.Common.Diagnostics
{
    public enum EventCategory
    {
        Unknown,
        Cache,
        Database,
        Page,
        Service,
        API,
        Authorization
    }
}
