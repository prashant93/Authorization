using Merilent.Authorization;
using System.Configuration;
using System.Net.Http.Formatting;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ClaimeAwareAPP_API
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Json Formatter
            GlobalConfiguration.Configuration.Formatters.Clear();
            GlobalConfiguration.Configuration.Formatters.Add(new JsonMediaTypeFormatter());

        }

        protected void Application_PostAuthenticateRequest()
        {
            //Adding Application Roles to  user

            ClaimsPrincipal currentPrincipal = ClaimsPrincipal.Current;
            Thread.CurrentPrincipal = RoleProvider.ApplicationRoles(currentPrincipal, ConfigurationManager.AppSettings["ida:APIName"], ConfigurationManager.ConnectionStrings["AuthorizationConnection"].ConnectionString);
            HttpContext.Current.User = Thread.CurrentPrincipal 
        }
    }
}