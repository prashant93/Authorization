using Merilent.Logger;
using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using System.Xml;

namespace Merilent.Authorization
{
    //Gets issuer name from sts
    internal class TokenIssuerHandler
    {
        public static string GetIssuer(string metadataEndpoint)
        {
            try
            {
                if (metadataEndpoint == null)
                {
                    var errorMessage = "ADFS MetadataEndpoint cannot be null, is used to get authenticated user claims";
                    LogHelper.Log($"{errorMessage}", LogLevel.ERROR);
                    throw new Exception(errorMessage);
                }
                LogHelper.Log("Token IssuerHandler called for fetching Valid issuer in the metadata", LogLevel.INFORMATION);

                string issuer = string.Empty;
                MetadataSerializer serializer = new MetadataSerializer()
                {
                    CertificateValidationMode = X509CertificateValidationMode.None,
                    TrustedStoreLocation = StoreLocation.CurrentUser,
                };
                MetadataBase metadata = serializer.ReadMetadata(XmlReader.Create(metadataEndpoint));
                EntityDescriptor entityDescriptor = (EntityDescriptor)metadata;
                if (!string.IsNullOrWhiteSpace(entityDescriptor.EntityId.Id))
                {
                    issuer = entityDescriptor.EntityId.Id;
                    LogHelper.Log($"ADFS Security Token Issuer is: {issuer}", LogLevel.INFORMATION);
                }
                return issuer;
            }
            catch (SecurityTokenException securityex)
            {
                var errorMessage = "The issuer of the token is not a trusted issuer.Remote certificate is invalid according to the validation procedure.";
                LogHelper.Log($"{errorMessage} : {securityex}", LogLevel.ERROR);
                throw new SecurityTokenException(errorMessage, securityex);
            }
            catch (FormatException formatex)
            {
                var errorMessage = "Cannot read serialize XMl format Metadata.Error reading the Federation metadata document";
                LogHelper.Log($"{errorMessage} : {formatex}", LogLevel.ERROR);
                throw new FormatException(errorMessage, formatex);
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to fetch Valid Issuer.The issuer of the security token was not recognized by the IssuerNameRegistry. To accept security tokens from this issuer, configure the IssuerNameRegistry to return a valid name for this issuer or Issuer is empty.";
                LogHelper.Log($"{errorMessage} : {ex}", LogLevel.ERROR);
                throw new Exception(errorMessage, ex);
            }
        }
    }
}