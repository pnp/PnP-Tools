// ------------------------------------------------------------------------------
//The MIT License(MIT)

//Copyright(c) 2015 Office Developer
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
// ------------------------------------------------------------------------------


using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Online.Applications.Core;
using Microsoft.Online.Applications.Core.Clients;
using System.Collections.Generic;
using System.Linq;


namespace TIP.Common.Services.Tenant.Internal
{
    internal class TenantManager : ActiveDirectoryAbstractService, ITenantInformationManager
    {
        #region Constructor
        internal TenantManager(AdalClient client)
        {
            this.Client = client;
        }
        #endregion
        public ITenantInformation GetTenantInformation()
        {
            var _client = this.GetActiveDirectoryClient();

            List<ITenantDetail> _tenantList = _client.TenantDetails
                .ExecuteAsync().Result.CurrentPage.ToList();

            var _tenantDetail = _tenantList.First();

            var _tenantInfo = new TenantInformation();
            _tenantInfo.DisplayName = _tenantDetail.DisplayName;
            _tenantInfo.TenantId = _tenantDetail.ObjectId;
            _tenantInfo.DirectorySyncEnabled = _tenantDetail.DirSyncEnabled;
            _tenantInfo.LastDirSyncTime = _tenantDetail.CompanyLastDirSyncTime;

            return _tenantInfo;
          
        }
    }
}
