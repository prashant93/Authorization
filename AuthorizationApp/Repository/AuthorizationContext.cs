using Authorization.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace Authorization.Repository
{
    public class AuthorizationContext : DbContext
    {
        public AuthorizationContext() : base("name = AuthorizationConnection")
        {
            Configuration.AutoDetectChangesEnabled = false;
            Database.SetInitializer<AuthorizationContext>(null);
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationRole> ApplicationRoles { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            modelBuilder.Entity<Application>().HasKey(a => a.Id);
            modelBuilder.Entity<Application>().Property(a => a.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<Application>().ToTable("tblApplication", "AtM");

            modelBuilder.Entity<ApplicationRole>().HasKey(d => d.ApplicationId);
            modelBuilder.Entity<ApplicationRole>().ToTable("vwApplicationRole", "At");

            base.OnModelCreating(modelBuilder);
        }
    }
}