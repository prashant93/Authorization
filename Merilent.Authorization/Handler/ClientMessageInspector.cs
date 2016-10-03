using Merilent.Logger;
using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Merilent.Authorization
{
    /// <summary>
    /// Adds token to every WCF calls
    /// </summary>
    public class ClientMessageInspector : IClientMessageInspector
    {
        private string _connectionString;
        private string _appName;
        private string _metadataEndpoint;
        private string _providerName;

        public ClientMessageInspector(string connectionString, string providerName, string appName, string metadataEndpoint)
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
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            //do nothing
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            try
            {
                var token = TokenGenerationHandler.GetToken(_connectionString, _appName, _metadataEndpoint);

                if (request.Properties.Count == 0 || request.Properties[HttpRequestMessageProperty.Name] == null)
                {
                    var property = new HttpRequestMessageProperty();
                    property.Headers["Authorization"] = token;
                    request.Properties.Add(HttpRequestMessageProperty.Name, property);
                }
                else
                {
                    ((HttpRequestMessageProperty)request.Properties[HttpRequestMessageProperty.Name]).Headers["Authorization"] = token;
                }

                return null;
            }
            catch (Exception ex)
            {
                var errorMessage = "User is not authenticated.Failed to Add header to request of service.";
                LogHelper.Log($"{errorMessage} : {ex}", LogLevel.ERROR);
                throw new Exception(errorMessage, ex);
            }
        }
    }

    /// <summary>
    /// Adds token to Message handler pipeline
    /// </summary>
    public class ClientHeaderBehavior : IEndpointBehavior
    {
        private ClientMessageInspector _clientMessageInspector;

        public ClientHeaderBehavior(string connectionString, string providerName, string appName, string metadataEndpoint)
        {
            _clientMessageInspector = new ClientMessageInspector(connectionString, providerName, appName, metadataEndpoint);
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            try
            {
                clientRuntime.ClientMessageInspectors.Add(_clientMessageInspector);
            }
            catch (Exception ex)
            {
                var error = $"Failed to generate  token";
                LogHelper.Log($"{error} : {ex}", LogLevel.ERROR);
                throw new Exception(error, ex);
            }
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
            // Nothing special here
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            // Nothing special here
        }

        public void Validate(ServiceEndpoint endpoint)
        {
            // Nothing special here
        }
    }

    /// <summary>
    /// Adds behavior for Message handler
    /// </summary>
    public class ClientBehaviorExtensionElement : BehaviorExtensionElement
    {
        private const string connectionString = "connectionString";
        private const string providerName = "providerName";
        private const string appName = "appName";
        private const string metadataEndpoint = "metadataEndpoint";

        public override Type BehaviorType
        {
            get { return typeof(ClientHeaderBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new ClientHeaderBehavior(this.ConnectionString, this.ProviderName, this.APPName, this.MetaDataEndpoint);
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
}