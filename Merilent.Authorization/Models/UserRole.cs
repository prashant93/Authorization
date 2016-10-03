using System.Collections.Generic;

namespace Authorization.Models
{
    public class UserRole
    {
        public List<string> ADRoles { get; set; }
        public List<string> ApplicationRoles { get; set; }
    }
}