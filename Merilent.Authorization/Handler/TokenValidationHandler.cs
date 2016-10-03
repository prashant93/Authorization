using Merilent.Logger;
using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Merilent.Authorization
{
    /// <summary>
    /// Validates a token for Web API
    /// </summary>
    public class TokenValidationHandler : DelegatingHandler
    {
        private string _metadataEndpoint;
        private string _appName;
        private string _apiSecret;
        private AuthContext _context;

        public TokenValidationHandler(string authConnectionString, string appName, string metadataEndpoint)
        {
            if (authConnectionString == null || appName == null || metadataEndpoint == null)
            {
                throw new Exception("ConnectionString, MetadataEndpoint or AppName cannot be null");
            }
            _metadataEndpoint = metadataEndpoint;
            _appName = appName;
            using (_context = new AuthContext(authConnectionString))
            {
                _apiSecret = (from v in _context.AppSecrets
                              where (v.ApplicationName == _appName && v.IsActive == true && v.ApplicationSecret != null)
                              select v.ApplicationSecret).First();
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(_apiSecret))
            {
                var message = "Secret key is empty";
                LogHelper.Log(message);
                throw new Exception(message + (new HttpResponseMessage(HttpStatusCode.InternalServerError).ToString()));
            }

            var secret = Encoding.UTF8.GetBytes(_apiSecret);
            string jwtToken;
            var tokenHandler = new JwtSecurityTokenHandler();

            using (HttpResponseMessage responseMessage = new HttpResponseMessage())
            {
                if (!TryRetrieveToken(request, out jwtToken))
                {
                    return base.SendAsync(request, cancellationToken);
                }
            }
            try
            {
                var handler = new JwtSecurityTokenHandler();
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
                    LogHelper.Log($"Claims for User {HttpContext.Current.User.Identity.Name} are:");
                    claimsPrincipal.Claims.
                        Select(x => new { x.Type, x.Value }).
                        ToList().ForEach(y => LogHelper.Log($"ClaimType{y.Type} => ClaimVlaue{y.Value}"));
                }
                return base.SendAsync(request, cancellationToken);
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

        private static bool TryRetrieveToken(HttpRequestMessage request, out string token)
        {
            token = null;
            if (!request.Headers.Contains("Authorization"))
            {
                var message = "Token Validation fails no Authorization Header for authorized request Your call is Anonymous";
                LogHelper.Log(message);
                return false;
            }
            string authzHeader = request.Headers.GetValues("Authorization").First<string>();

            token = authzHeader.StartsWith("Bearer ", StringComparison.Ordinal) ? authzHeader.Split(' ')[1] : null;
            if (null == token)
            {
                var message = "Token Validation fails no Authorization Header for authorized request Your call is Anonymous";
                LogHelper.Log(message);
                return false;
            }
            return true;
        }
    }
}