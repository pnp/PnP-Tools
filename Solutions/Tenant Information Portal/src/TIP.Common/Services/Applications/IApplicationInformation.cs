using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIP.Common.Services.Applications
{
    public interface IApplicationInformation
    {
        /// <summary>
        /// TODO
        /// </summary>
        string AppId { get; }

        /// <summary>
        /// TODO
        /// </summary>
        string DiplayName { get; }
       
        /// <summary>
        /// TODO
        /// </summary>
        DateTime? EndDate { get; }

        IList<string> ReplyUrls { get; }

        IList<string> IdentifierUris { get; }
    }
}
