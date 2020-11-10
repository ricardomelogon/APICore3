using System;
using WebApi.Authorization;
using WebApi.Data.Entities;

namespace WebApi.Data
{
    public static class EntitySeedData
    {
        public static EmailTemplate[] EmailTemplates()
        {
            return emailtemplates;
        }

        private readonly static EmailTemplate[] emailtemplates =
        {
            new EmailTemplate { Id = -2, Name = WebApi.EmailTemplates.Account.RegistrationCode, RowDate = DateTime.MinValue, Subject = "Welcome to WebApi", Body = "{0}" },
            new EmailTemplate { Id = -3, Name = WebApi.EmailTemplates.Account.ForgotPassword, RowDate = DateTime.MinValue, Subject = "WebApi - Forgot your password?", Body = "{0}" },
        };

        public static EmailConfig[] EmailConfigs()
        {
            return emailconfigs;
        }

        private readonly static EmailConfig[] emailconfigs =
        {
            new EmailConfig { Id = -1, Active = true, Disabled = false, IsDefaultReceiver = false, DisplayName = "DisplayName", Email = "sender@email.com", EnableSSL = true, Host = "smtp.host.cpom", IsDefaultSender = true, Password = "password", Port = 587, RowDate = DateTime.MinValue, UserName = "sender_username" },
            new EmailConfig { Id = -2, Active = true, Disabled = false, IsDefaultReceiver = true, DisplayName = "DisplayName", Email = "receiver@email.com", EnableSSL = true, Host = "imap.host.com", IsDefaultSender = false, Password = "password", Port = 993, RowDate = DateTime.MinValue, UserName = "receiver_username" }
        };

        public static Permissions[] Permissions()
        {
            return permissions;
        }

        private readonly static Permissions[] permissions =
        {
            new Permissions { Id = -1, Name = "AccessAll", Group="System", Value = (ushort)Permission.AccessAll, RowDate = DateTime.MinValue, Description = "This allows the user to access every feature and bypasses every restriction" },
            new Permissions { Id = -2, Name = "Employee", Group="System", Value = (ushort)Permission.User, RowDate = DateTime.MinValue, Description = "User" },
            new Permissions { Id = -3, Name = "Locked", Group="System", Value = (ushort)Permission.Locked, RowDate = DateTime.MinValue, Description = "Locked or Error" },
        };

        public static User[] Users()
        {
            return users;
        }

        private readonly static User[] users =
        {
            //Admin Password: Admin@1010
            new User { Id = new Guid("f8684da2-4887-4288-b841-af07477a54d1"), Email="admin@admin.com", FirstName = "WebApi", LastName = "Admin", EmailConfirmed = true, Enabled = true, LockoutEnabled = false, PasswordHash = "AQAAAAEAACcQAAAAEFP/5y7mPRDa2ZUjfLkwZ9M9kBq8f9gbHhuD7pdJxOO5SjT3kSVdexrbDNg0gUnRhw==", RowDate = DateTime.MinValue, AccessFailedCount = 0, NormalizedEmail = "ADMIN@ADMIN.COM", NormalizedUserName = "ADMIN@ADMIN.COM", SecurityStamp = "OYMY4LSEJV7NWVIQRYBDZ4FMP5F5BTCA", UserName = "admin@admin.com", RevokeCode = "b2f9f7e6-08c7-4ee8-8987-0416ffae2640" }
        };

        public static UserPermissions[] UserPermissions()
        {
            return userPermissions;
        }

        private readonly static UserPermissions[] userPermissions =
        {
            new UserPermissions{Id = -1, PermissionsId = -1, UserId = new Guid("f8684da2-4887-4288-b841-af07477a54d1"), RowDate = DateTime.MinValue },
            new UserPermissions{Id = -2, PermissionsId = -2, UserId = new Guid("f8684da2-4887-4288-b841-af07477a54d1"), RowDate = DateTime.MinValue }
        };

        public static Subscription[] Subscriptions()
        {
            return subscriptions;
        }

        private readonly static Subscription[] subscriptions =
        {
            new Subscription{ Id = 1, Date = DateTime.UtcNow.AddDays(30), Disabled = false, RowDate = DateTime.MinValue }
        };
    }
}