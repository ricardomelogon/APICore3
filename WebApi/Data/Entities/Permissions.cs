using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using WebApi.Authorization;

namespace WebApi.Data.Entities
{
    public class Permissions : IntBase
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public ushort Value { get; set; }

        [MaxLength(100)]
        public string Group { get; set; }

        [MaxLength(5)]
        public string Code { get; set; }

        [MaxLength(100)]
        public string ParentCodes { get; set; }

        [Column]
        [MaxLength(100)]
        public string Parents { get; private set; }

        public virtual ICollection<UserPermissions> Users { get; }

        [NotMapped]
        public IEnumerable<int> ParentIds
        {
            get => string.IsNullOrWhiteSpace(Parents) ? new List<int>() : Parents.Split(',').ToList().ConvertAll(s => int.Parse(s));
            set => Parents = value != null || value.Any() ? string.Join(",", value) : string.Empty;
        }

        [NotMapped]
        public Permission? Enum => this.Value.FindPermissionViaValue();
    }
}