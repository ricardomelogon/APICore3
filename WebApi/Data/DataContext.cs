using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using WebApi.Data.Entities;

namespace WebApi.Data
{
    public class DataContext : IdentityDbContext<User, Role, Guid>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Additional Configurations
            //Unique Attributes
            modelBuilder.Entity<EmailTemplate>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Permissions>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<User>().HasIndex(c => c.Email).IsUnique();
            modelBuilder.Entity<User>().HasIndex(c => c.UserName).IsUnique();
            //Unique Combination of Attributes
            modelBuilder.Entity<UserPermissions>().HasIndex(e => new { e.PermissionsId, e.UserId }).IsUnique();

            //Seed Data
            modelBuilder.Entity<EmailConfig>().HasData(EntitySeedData.EmailConfigs());
            modelBuilder.Entity<EmailTemplate>().HasData(EntitySeedData.EmailTemplates());
            modelBuilder.Entity<Permissions>().HasData(EntitySeedData.Permissions());
            modelBuilder.Entity<User>().HasData(EntitySeedData.Users());
            modelBuilder.Entity<UserPermissions>().HasData(EntitySeedData.UserPermissions());
            modelBuilder.Entity<Subscription>().HasData(EntitySeedData.Subscriptions());
        }

        public DbSet<EmailConfig> EmailConfigs { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<Permissions> Permissions { get; set; }
        public DbSet<UserPermissions> UserPermissions { get; set; }
        public DbSet<Subscription> Subscription { get; set; }
    }
}