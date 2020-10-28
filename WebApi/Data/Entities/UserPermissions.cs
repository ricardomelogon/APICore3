using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Data.Entities
{
    public class UserPermissions : IntBase
    {
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        public virtual User User { get; set; }

        [ForeignKey(nameof(Permissions))]
        public int PermissionsId { get; set; }

        public virtual Permissions Permissions { get; set; }
    }
}