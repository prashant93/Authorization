using Merilent.Authorization.Models;
using System.Data.Entity;

namespace Merilent.Authorization
{
    public class AuthContext : DbContext
    {
        public AuthContext(string Connectionstring) : base(Connectionstring)
        {
            Configuration.AutoDetectChangesEnabled = false;
            Database.SetInitializer<AuthContext>(null);
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<AppSecret> AppSecrets { get; set; }
        public DbSet<Permission> PermissionViews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppSecret>().HasKey(d => d.Id);
            modelBuilder.Entity<AppSecret>().ToTable("tblApplication", "AtM");

            modelBuilder.Entity<Permission>().HasKey(d => d.ApplicationId);
            modelBuilder.Entity<Permission>().ToTable("vwApplicationPermission", "At");

            base.OnModelCreating(modelBuilder);
        }
    }
}