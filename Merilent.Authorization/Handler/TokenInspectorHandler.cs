using Merilent.Logger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Security;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading;
using System.Web;

namespace Merilent.Authorization
{
    /// <summary>
    /// Validates a token received from WCF client
    /// </summary>
    public class TokenInspectorHandler : IDispatchMessageInspector
    {
        private string _connectionString;
        private string _providerName;
        private string _appName;
        private string _metadataEndpoint;
        private string _apiSecret;
        private AuthContext _context;

        public TokenInspectorHandler(string connectionString, string providerName, string appName, string metadataEndpoint)
        {
            if (connectionString == null || appName == null || metadataEndpoint == null)
            {
                var message = "ConnectionString, MetadataEndpoint or AppName cannot be null";
                LogHelper.Log(message);
                throw new Exception(message);
            }
            _connectionString = connectionString;
            _providerName = providerName;
            _appName = appName;
            _metadataEndpoint = metadataEndpoint;
            _context = new AuthContext(_connectionString);
            _apiSecret = (from v in _context.AppSecrets
                          where (v.ApplicationName == _appName && v.IsActive == true && v.ApplicationSecret != null)
                          select v.ApplicationSecret).First();
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            object correlationState = null;
            try
            {
                HttpRequestMessageProperty requestMessage = request.Properties["httpRequest"] as HttpRequestMessageProperty;
                if (request == null)
                {
                    throw new InvalidOperationException("Invalid request type.");
                }
                string authHeader = requestMessage.Headers["Authorization"];

                if (string.IsNullOrEmpty(authHeader) || !this.Authenticate(authHeader))
                {
                    var message = "Token Validation fails no Authorization Header for authorized request Your call is Anonymous";
                    LogHelper.Log(message);
                    return false;
                    //string error = "Authorization header is empty";
                    //correlationState = error;
                    //LogHelper.Log("Access is denied for user", LogLevel.ERROR);
                    //LogHelper.Log(new System.ServiceModel.Security.SecurityAccessDeniedException("Access denied").ToString(), LogLevel.ERROR);

                    //var claims = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.Select(g => new Claim(g.Type, g.Value)).ToList();
                    //string claim = string.Join<Claim>(",", claims.ToArray());
                    //throw new SecurityAccessDeniedException($"Access denied for user : { HttpContext.Current.User.Identity.Name} and claims {claim}");
                }
                return correlationState;
            }
            catch (SecurityAccessDeniedException secex)
            {
                var errorMessage = "Unauthorized to perform this Operation";
                LogHelper.Log($"{HttpContext.Current.User.Identity.Name}-{errorMessage} : {secex}", LogLevel.ERROR);
                throw new Exception(errorMessage, secex);
            }
            catch (Exception ex)
            {
                LogHelper.Log($"{ex.Message} : {ex}", LogLevel.ERROR);
                throw new Exception(ex.Message, ex);
            }
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            WcfErrorResponseData error = correlationState as WcfErrorResponseData;
            if (error != null)
            {
                HttpResponseMessageProperty responseProperty = new HttpResponseMessageProperty();
                reply.Properties["httpResponse"] = responseProperty;
                responseProperty.StatusCode = error.StatusCode;

                IList<KeyValuePair<string, string>> headers = error.Headers;
                if (headers != null)
                {
                    for (int i = 0; i < headers.Count; i++)
                    {
                        responseProperty.Headers.Add(headers[i].Key, headers[i].Value);
                    }
                }
            }
        }

        private bool Authenticate(string authHeader)
        {
            var secret = Encoding.UTF8.GetBytes(_apiSecret);
            var handler = new JwtSecurityTokenHandler();
            string jwtToken = authHeader;
            var validationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidIssuer = TokenIssuerHandler.GetIssuer(_metadataEndpoint),
                IssuerSigningToken = new BinarySecretSecurityToken(secret),
            };
            SecurityToken validatedToken;
            ClaimsPrincipal claimsPrincipal = handler.ValidateToken(jwtToken, validationParameters, out validatedToken);
            Thread.CurrentPrincipal = claimsPrincipal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = claimsPrincipal;
                return true;
            }
            else
            {
                throw new Exception("Unable to assign User to current context. Please enable <serviceHostingEnvironment aspNetCompatibilityEnabled='true' /> in WCF web.config.");
            }
            //if (OperationContext.Current != null)
            //{
            //    //OperationContext.Current.u
            //    Thread.CurrentPrincipal = claimsPrincipal;

            //    return true;
            //}
            //else if (HttpContext.Current != null)
            //{
            //    HttpContext.Current.User = claimsPrincipal;
            //    return true;
            //}
        }
    }

    /// <summary>
    /// Add Token Validaiton to Message handler pipelin
    /// </summary>
    public class BearerTokenServiceBehavior : IServiceBehavior
    {
        private TokenInspectorHandler _tokenInspector;

        public BearerTokenServiceBehavior(string connectionString, string providerName, string appName, string metadataEndpoint)
        {
            _tokenInspector = new TokenInspectorHandler(connectionString, providerName, appName, metadataEndpoint);
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
            // no-op
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            try
            {
                foreach (ChannelDispatcher chDisp in serviceHostBase.ChannelDispatchers)
                {
                    foreach (EndpointDispatcher epDisp in chDisp.Endpoints)
                    {//change this
                        epDisp.DispatchRuntime.MessageInspectors.Add(_tokenInspector);
                    }
                }
            }
            catch (SecurityTokenValidationException stex)
            {
                var errorMessage = " User is not authenticated ,token Validation failed ";
                LogHelper.Log($"{errorMessage} : {stex}", LogLevel.ERROR);
                throw new SecurityTokenValidationException(errorMessage, stex);
            }
            catch (SecurityTokenException secex)
            {
                var errorMessage = "Access is denied for user to perform this operation";
                LogHelper.Log($"{errorMessage} : {secex}", LogLevel.ERROR);
                throw new SecurityTokenException(errorMessage, secex);
            }
            catch (Exception ex)
            {
                var errorMessage = "Token Validation failed";
                LogHelper.Log($"{errorMessage} : {ex}", LogLevel.ERROR);
                throw new Exception(errorMessage, ex);
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            // no-op
        }
    }

    /// <summary>
    /// Adds behavior for Token validation
    /// </summary>
    public class BearerTokenExtensionElement : BehaviorExtensionElement
    {
        private const string connectionString = "connectionString";
        private const string providerName = "providerName";

        private const string appName = "appName";
        private const string metadataEndpoint = "metadataEndpoint";

        public override Type BehaviorType
        {
            get { return typeof(BearerTokenServiceBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new BearerTokenServiceBehavior(this.ConnectionString, this.ProviderName, this.APPName, this.MetaDataEndpoint);
        }

        [ConfigurationProperty(connectionString, IsRequired = true)]
        public string ConnectionString
        {
            get { return this[connectionString] as string; }
            set { this[connectionString] = value; }
        }

        [ConfigurationProperty(providerName, IsRequired = true)]
        public string ProviderName
        {
            get { return this[providerName] as string; }
            set { this[providerName] = value; }
        }

        [ConfigurationProperty(appName, IsRequired = true)]
        public string APPName
        {
            get { return this[appName] as string; }
            set { this[appName] = value; }
        }

        [ConfigurationProperty(metadataEndpoint, IsRequired = true)]
        public string MetaDataEndpoint
        {
            get { return this[metadataEndpoint] as string; }
            set { this[metadataEndpoint] = value; }
        }
    }

    /// <summary>
    /// Wcf Error
    /// </summary>
    internal class WcfErrorResponseData
    {
        public WcfErrorResponseData(HttpStatusCode status) :
              this(status, string.Empty, new KeyValuePair<string, string>[0])
        {
        }

        public WcfErrorResponseData(HttpStatusCode status, string body) :
              this(status, body, new KeyValuePair<string, string>[0])
        {
        }

        public WcfErrorResponseData(HttpStatusCode status, string body, params KeyValuePair<string, string>[] headers)
        {
            StatusCode = status;
            Body = body;
            Headers = headers;
        }

        public HttpStatusCode StatusCode
        {
            private set;
            get;
        }

        public string Body
        {
            private set;
            get;
        }

        public IList<KeyValuePair<string, string>> Headers
        {
            private set;
            get;
        }
    }
}