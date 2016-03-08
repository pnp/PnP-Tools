using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace TIP.Common.Services.Applications.Internal
{
    internal class ApplicationInformation : IApplicationInformation
    {
        #region Private Instance Members
        private string _appID;
        private string _displayName;
        private DateTime? _endDate;
        private IList<string> _identifierUris = new List<string>();
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

        [JsonProperty(PropertyName = "replyUrls")]
        public IList<string> ReplyUrls
        {
            get
            {
                return this._replyUrls;
            }
            internal set { this._replyUrls = value; }
        }

        [JsonProperty(PropertyName = "identifierUris")]
        public IList<string> IdentifierUris
        {
            get
            {
                return this._identifierUris;
            }
            internal set { this._identifierUris = value; }
        }
    }
}
