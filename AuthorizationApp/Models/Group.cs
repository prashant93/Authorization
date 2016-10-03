using System;
using System.Web;

namespace Authorization.Models
{
    public class Group
    {
        public Group()
        {
            CreatedOn = DateTime.Now;
            if (HttpContext.Current.User.Identity.IsAuthenticated)
                CreatedBy = HttpContext.Current.User.Identity.Name;
            else
                CreatedBy = $"{Environment.UserDomainName}\\{Environment.UserName}";

            IsActive = true;
        }

        public int Id { get; set; }
        public string GroupName { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }
    }
}