using Merilent.Logger;
using System;
using System.IdentityModel.Protocols.WSTrust;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Merilent.Authorization
{
    /// <summary>
    /// Creates token for Authenticated user
    /// </summary>
    public class TokenGenerationHandler
    {
        public static string GetToken(string authConnectionString, string appName, string metadataEndpoint)
        {
            try
            {
                string tokenString = string.Empty;

                if (authConnectionString == null || appName == null || metadataEndpoint == null)
                {
                    var message = "ConnectionString, MetadataEndpoint or AppName cannot be null";
                    LogHelper.Log(message);
                    throw new Exception(message);
                }
                var tokenHandler = new JwtSecurityTokenHandler();
                var symmetricKey = Encoding.UTF8.GetBytes(SecretHandler.GetAPISecret(authConnectionString, appName));

                var claims = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.Select(g => new Claim(g.Type, g.Value)).ToList();
                var user = HttpContext.Current.User.Identity.Name;
                var role = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role)
                                            .Union(((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.Where(c => c.Type == ClaimTypes.Upn))
                                            .Select(c => c.Value).ToList();
                var applicationClaims = ApplicationClaimsHandler.GetApplicationClaims(authConnectionString, appName, role);

                if (applicationClaims != null) { claims.AddRange(applicationClaims); }

                LogHelper.Log($"Token GenerationHandler called for generating JWT token for Authenticated User : {user} ", LogLevel.INFORMATION);

                if (HttpContext.Current.User.Identity != null)
                {
                    LogHelper.Log($"Generating JWT token for  User:{user} ", LogLevel.INFORMATION);
                }

                if (claims != null)
                {
                    var now = DateTime.UtcNow;
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims.ToArray()),
                        TokenIssuerName = TokenIssuerHandler.GetIssuer(metadataEndpoint),
                        Lifetime = new Lifetime(now, now.AddMinutes(10)),
                        SigningCredentials = new SigningCredentials(
                                new InMemorySymmetricSecurityKey(symmetricKey),
                                "http://www.w3.org/2001/04/xmldsig-more#hmac-sha256",
                                "http://www.w3.org/2001/04/xmlenc#sha256"),
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    tokenString = tokenHandler.WriteToken(token);
                    LogHelper.Log($"JWT token generated sucessfully for  User:{user} ", LogLevel.INFORMATION);
                }

                return tokenString;
            }
            catch (WebException webex)
            {
                var errorMessage = $"The remote server returned {webex.Status}";
                LogHelper.Log($"{errorMessage} {webex} ", LogLevel.ERROR);
                throw new WebException(errorMessage, webex);
            }
            catch (SecurityTokenException securitytoken)
            {
                var errorMessage = $"Not able to connect {authConnectionString} and get secret key for {appName}:";
                throw new InvalidOperationException(errorMessage, securitytoken);
            }
            catch (FormatException formatexception)
            {
                var errorMessage = $"Not able to connect {authConnectionString} and get secret key for {appName}:";
                throw new InvalidOperationException(errorMessage, formatexception);
            }
            catch (Exception ex)
            {
                var error = $"Failed to generate JWT token";
                LogHelper.Log($"{error} : {ex}", LogLevel.ERROR);
                throw new Exception(error, ex);
            }
        }
    }
}