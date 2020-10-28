using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Data.Entities
{
    public partial interface IBase<T>
    {
        DateTime RowDate { get; set; }
        T Id { get; set; }
        bool Disabled { get; set; }
    }

    public class GuidBase : IBase<Guid>
    {
        public Guid Id { get; set; }
        public DateTime RowDate { get; set; }
        public bool Disabled { get; set; }
    }

    public class IntBase : IBase<int>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public DateTime RowDate { get; set; }
        public bool Disabled { get; set; }
    }
}