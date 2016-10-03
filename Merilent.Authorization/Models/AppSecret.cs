namespace Merilent.Authorization.Models
{
    public class AppSecret
    {
        public int Id { get; set; }
        public string ApplicationName { get; set; }
        public string ApplicationSecret { get; set; }
        public bool IsActive { get; set; }
    }
}