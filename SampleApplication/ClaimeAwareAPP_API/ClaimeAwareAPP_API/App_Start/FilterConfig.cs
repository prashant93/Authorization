using System.Web.Mvc;

namespace ClaimeAwareAPP_API
{
    public class FilterConfig : ActionFilterAttribute
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}