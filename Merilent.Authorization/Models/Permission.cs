namespace Merilent.Authorization.Models
{
    public class Permission
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; }
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
    }
}