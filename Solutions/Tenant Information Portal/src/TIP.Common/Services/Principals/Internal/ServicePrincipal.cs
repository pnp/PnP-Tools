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
using System.Collections.Generic;

namespace TIP.Common.Services.Principals.Internal
{
    /// <summary>
    /// Domain Object that repesents a Service Principal for add-ins and Apps that are 
    /// registered in ACS/Azure Active Directory
    /// </summary>
    internal class ServicePrincipal : IAzureServicePrincipal
    {
        #region Private Instance Members
        private string _appID;
        private string _displayName;
        private DateTime? _endDate;
        private IList<string> _principalNames = new List<string>();
        private IList<string> _replyUrls = new List<string>();
        #endregion

        #region Properties
        [JsonProperty(PropertyName = "appId")]
        public string AppId
        {
            get
            {
                return this._appID;
            }
            internal set
            {
                this._appID = value;
            }
        }

        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName
        {
            get
            {
                return this._displayName;
            }
            internal set { this._displayName = value; }
        }
        [JsonProperty(PropertyName = "endDate")]
        public DateTime? EndDate
        {
            get
            {
                return this._endDate;
            }
            set { this._endDate = value; }
        }
        [JsonProperty(PropertyName = "principalNames")]
        public IList<string> PrincipalNames
        {
            get
            {
                return this._principalNames;
            }
            internal set { this._principalNames = value; }
        }

        [JsonProperty(PropertyName = "replyUrls")]
        public IList<string> ReplyUrls
        {
            get
            {
                return this._replyUrls;
            }
            internal set { this._replyUrls = value; }
        }
        #endregion
    }
}
