namespace SearchQueryTool.SPAuthenticationClient
{
    using System;
    using System.Runtime.Serialization;

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.Runtime.Serialization.DataContractAttribute(Name = "LoginResult", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    [System.SerializableAttribute()]
    public partial class LoginResult : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged
    {
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;

        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string CookieNameField;

        private LoginErrorCode ErrorCodeField;

        private int TimeoutSecondsField;

        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData
        {
            get
            {
                return this.extensionDataField;
            }
            set
            {
                this.extensionDataField = value;
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false)]
        public string CookieName
        {
            get
            {
                return this.CookieNameField;
            }
            set
            {
                if ((object.ReferenceEquals(this.CookieNameField, value) != true))
                {
                    this.CookieNameField = value;
                    this.RaisePropertyChanged("CookieName");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public LoginErrorCode ErrorCode
        {
            get
            {
                return this.ErrorCodeField;
            }
            set
            {
                if ((this.ErrorCodeField.Equals(value) != true))
                {
                    this.ErrorCodeField = value;
                    this.RaisePropertyChanged("ErrorCode");
                }
            }
        }

        [System.Runtime.Serialization.DataMemberAttribute(IsRequired = true)]
        public int TimeoutSeconds
        {
            get
            {
                return this.TimeoutSecondsField;
            }
            set
            {
                if ((this.TimeoutSecondsField.Equals(value) != true))
                {
                    this.TimeoutSecondsField = value;
                    this.RaisePropertyChanged("TimeoutSeconds");
                }
            }
        }

        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null))
            {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [System.Runtime.Serialization.DataContractAttribute(Name = "LoginErrorCode", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public enum LoginErrorCode : int
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        NoError = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        NotInFormsAuthenticationMode = 1,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        PasswordNotMatch = 2,
    }

    [System.Runtime.Serialization.DataContractAttribute(Name = "AuthenticationMode", Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public enum AuthenticationMode : int
    {

        [System.Runtime.Serialization.EnumMemberAttribute()]
        None = 0,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Windows = 1,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Passport = 2,

        [System.Runtime.Serialization.EnumMemberAttribute()]
        Forms = 3,
    }

    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://schemas.microsoft.com/sharepoint/soap/", ConfigurationName = "SPAuthenticationServiceReference.AuthenticationSoap")]
    public interface AuthenticationSoap
    {
        [System.ServiceModel.OperationContractAttribute(Action = "http://schemas.microsoft.com/sharepoint/soap/Login", ReplyAction = "*")]
        LoginResponse Login(LoginRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://schemas.microsoft.com/sharepoint/soap/Login", ReplyAction = "*")]
        System.Threading.Tasks.Task<LoginResponse> LoginAsync(LoginRequest request);

        [System.ServiceModel.OperationContractAttribute(Action = "http://schemas.microsoft.com/sharepoint/soap/Mode", ReplyAction = "*")]
        AuthenticationMode Mode();

        [System.ServiceModel.OperationContractAttribute(Action = "http://schemas.microsoft.com/sharepoint/soap/Mode", ReplyAction = "*")]
        System.Threading.Tasks.Task<AuthenticationMode> ModeAsync();
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class LoginRequest
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "Login", Namespace = "http://schemas.microsoft.com/sharepoint/soap/", Order = 0)]
        public LoginRequestBody Body;

        public LoginRequest()
        {
        }

        public LoginRequest(LoginRequestBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public partial class LoginRequestBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public string username;

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 1)]
        public string password;

        public LoginRequestBody()
        {
        }

        public LoginRequestBody(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped = false)]
    public partial class LoginResponse
    {

        [System.ServiceModel.MessageBodyMemberAttribute(Name = "LoginResponse", Namespace = "http://schemas.microsoft.com/sharepoint/soap/", Order = 0)]
        public LoginResponseBody Body;

        public LoginResponse()
        {
        }

        public LoginResponse(LoginResponseBody Body)
        {
            this.Body = Body;
        }
    }

    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace = "http://schemas.microsoft.com/sharepoint/soap/")]
    public partial class LoginResponseBody
    {

        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue = false, Order = 0)]
        public LoginResult LoginResult;

        public LoginResponseBody()
        {
        }

        public LoginResponseBody(LoginResult LoginResult)
        {
            this.LoginResult = LoginResult;
        }
    }

    public interface AuthenticationSoapChannel : AuthenticationSoap, System.ServiceModel.IClientChannel
    {
    }

    public partial class AuthenticationSoapClient : System.ServiceModel.ClientBase<AuthenticationSoap>, AuthenticationSoap
    {

        public AuthenticationSoapClient()
        {
        }

        public AuthenticationSoapClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public AuthenticationSoapClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public AuthenticationSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public AuthenticationSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        LoginResponse AuthenticationSoap.Login(LoginRequest request)
        {
            return base.Channel.Login(request);
        }

        public LoginResult Login(string username, string password)
        {
            LoginRequest inValue = new LoginRequest();
            inValue.Body = new LoginRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            LoginResponse retVal = ((AuthenticationSoap)(this)).Login(inValue);
            return retVal.Body.LoginResult;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<LoginResponse> AuthenticationSoap.LoginAsync(LoginRequest request)
        {
            return base.Channel.LoginAsync(request);
        }

        public System.Threading.Tasks.Task<LoginResponse> LoginAsync(string username, string password)
        {
            LoginRequest inValue = new LoginRequest();
            inValue.Body = new LoginRequestBody();
            inValue.Body.username = username;
            inValue.Body.password = password;
            return ((AuthenticationSoap)(this)).LoginAsync(inValue);
        }

        public AuthenticationMode Mode()
        {
            return base.Channel.Mode();
        }

        public System.Threading.Tasks.Task<AuthenticationMode> ModeAsync()
        {
            return base.Channel.ModeAsync();
        }
    }
}
