using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.Entities
{
    public class User : IdentityUser<Guid>
    {
        [MaxLength(100)]
        public string FirstName { get; set; }

        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string ConfirmationCode { get; set; }

        public DateTime RowDate { get; set; }

        public bool Enabled { get; set; } = true;

        [MaxLength(40)]
        public string RevokeCode { get; set; }

        public virtual ICollection<UserPermissions> Permissions { get; }

        public bool IsLocked()
        {
            return !Enabled || (LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow);
        }
    }
}