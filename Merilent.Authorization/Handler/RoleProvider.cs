using Merilent.Logger;
using System;
using System.Linq;
using System.Security.Claims;

namespace Merilent.Authorization
{
    /// <summary>
    ///  Add Applicatin roles to user pricipal
    /// </summary>
    public class RoleProvider
    {
        public static ClaimsPrincipal ApplicationRoles(ClaimsPrincipal incomingPrincipal, string applicationname, string authConnectionString)
        {
            try
            {
                if (!incomingPrincipal.Identity.IsAuthenticated)
                {
                    return incomingPrincipal;
                }
                LogHelper.Log("Adding ApplicationRoles to User");
                var role = incomingPrincipal.Claims.Where(c => c.Type == ClaimTypes.Role)
                                            .Union(incomingPrincipal.Claims.Where(c => c.Type == ClaimTypes.Upn))
                                            .Select(c => c.Value).ToList();
                var applicationClaims = ApplicationClaimsHandler.GetApplicationClaims(authConnectionString, applicationname, role);
                applicationClaims.AddRange(incomingPrincipal.Claims);
                return new ClaimsPrincipal(new ClaimsIdentity(applicationClaims, AuthenticationTypes.Federation));
            }
            catch (Exception ex)
            {
                var errorMessage = "Failed to add Application roles.";
                LogHelper.Log($"{errorMessage} : {ex}", LogLevel.ERROR);
                throw new Exception(errorMessage, ex);
            }
        }
    }
}