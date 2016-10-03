using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Authorization.Models
{
    public class ApplicationPermission
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public int? GroupId { get; set; }
        public string GroupName { get; set; }
        public int? UserId { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}