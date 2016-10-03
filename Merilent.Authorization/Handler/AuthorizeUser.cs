using Merilent.Logger;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Merilent.Authorization
{
    /// <summary>
    /// Overrides AuthorizeAttribute for handling Custom Message
    /// </summary>
    ///

    public class AuthorizeUser : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            if (Thread.CurrentPrincipal.Identity.IsAuthenticated)
            {
                LogHelper.Log($"{HttpContext.Current.User.Identity.Name} is Authenticated");
                if (!IsAuthorized(actionContext))
                {
                    LogHelper.Log($"{HttpContext.Current.User.Identity.Name} is not Authorized");
                    var role = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.Where(c => c.Type == ClaimTypes.Role)
                                          .Select(c => c.Value).ToList();
                    var message = ($"User {HttpContext.Current.User.Identity.Name} do not have permission to perform this operation. <br/>User Roles: {string.Join(", ", role.ToArray())}. <br/><br/>For Access, Please contact Admininstrator.");
                    LogHelper.Log(message);
                    actionContext.Response = new HttpResponseMessage()
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        Content = new StringContent(message)
                    };
                    throw new Exception(message);
                }
            }
            else
            {
                LogHelper.Log($"User is not Authenticated");
                actionContext.Response = new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.Unauthorized,
                };
                throw new Exception("User is UnAuthenticated.Please Login to Applciation to access.");
            }
        }
    }
}