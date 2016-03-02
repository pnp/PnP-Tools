using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TIP.Common.Services.Applications.Internal
{
    internal class ApplicationInformation : IApplicationInformation
    {
        #region Private Instance Members
        private string _appID;
        private string _displayName;
        private DateTime? _endDate;
        private IList<string> _principalNames = new List<string>();
        private IList<string> _replyUrls = new List<string>();
        #endregion

        [JsonProperty(PropertyName = "appId")]
        public string AppId
        {
            get
            {
                return this._appID;
            }
            set
            {
                this._appID = value;
            }
        }

        [JsonProperty(PropertyName = "displayName")]
        public string DiplayName
        {
            get
            {
                return this._displayName;
            }
            set { this._displayName = value; }
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
    }
}
