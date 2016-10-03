using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Authorization.Helper
{
    public class Default
    {
        public Default()
        {
            CreatedOn = DateTime.Now;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                CreatedBy = HttpContext.Current.User.Identity.Name;
            else
                CreatedBy = $"{Environment.UserDomainName}\\{Environment.UserName}";

            IsActive = true;
        }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}