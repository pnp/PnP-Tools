using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace PnPAutomationUI
{
    /// <summary> 
    /// Partial class to offer a connectionstring override 
    /// </summary> 
    public partial class PnPTestAutomationEntities : DbContext
    {
        public PnPTestAutomationEntities(string connectionString)
             : base(connectionString) 
         {
        }
    }

}