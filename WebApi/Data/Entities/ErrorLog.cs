using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Data.Entities
{
    public class ErrorLog : IntBase
    {
        [MaxLength(2000)]
        public string Log { get; set; }

        [MaxLength(100)]
        public string Method { get; set; }

        [MaxLength(500)]
        public string Path { get; set; }

        [ForeignKey(nameof(User))]
        public Guid? UserId { get; set; }

        public virtual User User { get; set; }
    }
}