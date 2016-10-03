using Merilent.Authorization;
using Merilent.Logger;
using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace ClaimeAwareAPP_API.Controllers
{
    public class PMController : ApiController
    {
        [AllowAnonymous]
        public string Get()
        {
            LogHelper.Log("Anonymous User added new PM");
            return "Anonymous User added new PM";
        }

        [AuthorizeUser(Roles = "Admin")]
        public string Get(int id)
        {
            try
            {
                var message = $"{HttpContext.Current.User.Identity.Name} deleted record for PM ID : {id} successfully ";
                LogHelper.Log(message);
                return message;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}