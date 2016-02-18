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



using Newtonsoft.Json;
using System;

namespace TIP.Common.Services.Tenant.Internal
{
    internal class TenantInformation : ITenantInformation
    {
        #region instanceMembers
        private string _displayName;
        private string _tenantID;
        private bool? _dirSyncEnabled;
        private DateTime? _lastDirSyncTime;

        #endregion
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName
        {
            get { return this._displayName; }
            internal set { this._displayName = value; }
        }

        [JsonProperty(PropertyName = "tenantId")]
        public string TenantId
        {
            get { return this._tenantID; }
            internal set { this._tenantID = value; }
        }
        [JsonProperty(PropertyName = "directorySyncEnabled")]
        public bool? DirectorySyncEnabled
        {
            get { return this._dirSyncEnabled; }
            internal set { this._dirSyncEnabled = value; }
        }

        [JsonProperty(PropertyName = "lastDirSyncTime")]
        public DateTime? LastDirSyncTime
        {
            get { return this._lastDirSyncTime; }
            internal set { this._lastDirSyncTime = value; }
        }
    }
}
