namespace Authorization.Models
{
    public class ApplicationRole
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationURL { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}