using Authorization.Models;
using Merilent.Logger;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;

namespace Merilent.Authorization
{
    /// <summary>
    /// Gets Authenticated user Roles
    /// </summary>
    public class UserRolesHandler
    {
        private static string _connectionstring;

        public UserRolesHandler()
        {
        }

        public UserRolesHandler(string connectionstring)
        {
            _connectionstring = connectionstring;
        }

        public List<string> GetUserRoles()
        {
            var roles = ClaimsPrincipal.Current.Claims.Where(c => c.Type == ClaimTypes.Role)
                                            .Union(ClaimsPrincipal.Current.Claims.Where(c => c.Type == ClaimTypes.Upn))
                                            .Select(c => c.Value).ToList();
            // var userRoles = string.Join(",", roles.ToArray());
            return roles;
        }

        public UserRole GetRoles(string userName, string applicationame, string domainName)
        {
            try
            {
                if (userName == null || applicationame == null || domainName == null)
                {
                    throw new Exception("Argument username, applicationname or domainname cannot be Empty");
                }
                UserRole roles = new UserRole();
                roles.ADRoles = new List<string>();
                roles.ApplicationRoles = new List<string>();

                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domainName))
                {
                    if (context != null)
                    {
                        using (UserPrincipal userPrincipal = UserPrincipal.FindByIdentity(context, userName))
                        {
                            LogHelper.Log($"Getting roles for user: {userName} for application: {applicationame}", LogLevel.INFORMATION);
                            // find the user userName  e.g. ppanigrahi or ppanigrahi@merilent.com, Recommended ppanigrahi@merilent.com

                            if (userPrincipal != null)
                            {
                                var authGroups = userPrincipal.GetGroups().ToList();  // get groups for the user
                                var upn = userPrincipal.UserPrincipalName.ToString(); // get upn or email address for user

                                //roles.Add(upn);

                                // add ad groups as role
                                foreach (var authGroup in authGroups)
                                {
                                    var role = authGroup.ToString();

                                    roles.ADRoles.Add(role);
                                }

                                var applicationRoles = ApplicationClaimsHandler.GetApplicationClaims(_connectionstring, applicationame, roles.ADRoles);
                                if (applicationRoles != null)
                                {
                                    // add application roles to user
                                    foreach (var applicationRole in applicationRoles)
                                    {
                                        var role = applicationRole.Value.ToString();
                                        roles.ApplicationRoles.Add(role);
                                    }
                                }
                                LogHelper.Log($"User: {userName} have roles: {roles} for {applicationame}", LogLevel.INFORMATION);
                            }
                            else
                            {
                                throw new Exception($"User <strong>{userName}</strong> is not part of Domain: <strong>{domainName}</strong>. <br> Check  User  <strong>{userName}</strong> is valid ");
                            }
                        }
                    }
                    else
                    {
                        throw new Exception($" You passed domain name as <strong>{domainName}</strong>. <br> Check Domain is not correct or user don't have access to Domain.");
                    }

                    return roles;
                }
            }
            catch (PrincipalServerDownException ex)
            {
                throw new PrincipalServerDownException($" You passed domain name as <strong>{domainName}</strong>. <br> Check Domain is not correct or user don't have access to Domain.", ex);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}