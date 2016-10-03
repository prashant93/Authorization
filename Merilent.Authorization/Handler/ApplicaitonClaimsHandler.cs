using Merilent.Logger;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;

namespace Merilent.Authorization
{
    /// <summary>
    /// Adds Roles to a Group
    /// </summary>
    public class ApplicationClaimsHandler
    {
        private static AuthContext _context;

        public static List<Claim> GetApplicationClaims(string authConnectionString, string applicationname, List<string> groups)
        {
            try
            {
                List<Claim> claims = new List<Claim>();

                var s = new List<string>();

                using (_context = new AuthContext(authConnectionString))
                {
                    foreach (var group in groups)
                    {
                        var roles = _context.PermissionViews
                                            .Where(x => ((x.GroupName == group && x.GroupName != null) ||
                                                        (x.UserName == group && x.UserName != null)) &&
                                                         x.ApplicationName == applicationname)
                                            .Select(y => y.RoleName).ToList();

                        foreach (var role in roles)
                        {
                            if (role != null)
                            {
                                claims.Add(new Claim(ClaimTypes.Role, role));
                            }
                        }
                    }
                }

                LogHelper.Log($"User belongs to :{string.Join<Claim>(",\n", claims.ToArray())}");
                return claims;
            }
            catch (SqlException sqlex)
            {
                var errorMessage = $"Cannot open connection to {authConnectionString}";
                LogHelper.Log($"{errorMessage} {sqlex}", LogLevel.ERROR);
                throw new Exception(errorMessage, sqlex);
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