using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIP.Common.Services.Applications
{
    public interface IApplicationManager
    {
        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        List<IApplicationInformation> GetAllApplications();

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IApplicationInformation GetApplicationById(string id);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="numberOfDays"></param>
        /// <returns></returns>
        IList<IApplicationInformation> GetExpiredApplicationInDays(double numberOfDays);

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        IList<IApplicationInformation> GetAllExpired();

    }
}
