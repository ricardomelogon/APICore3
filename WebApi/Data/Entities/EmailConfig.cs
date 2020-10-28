using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.Entities
{
    public class EmailConfig : IntBase
    {
        [MaxLength(100)]
        public string DisplayName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Password { get; set; }

        [MaxLength(100)]
        public string Host { get; set; }

        [MaxLength(100)]
        public int Port { get; set; }

        [MaxLength(100)]
        public string UserName { get; set; }

        public bool IsDefaultReceiver { get; set; }

        public bool EnableSSL { get; set; }
        public bool IsDefaultSender { get; set; }
        public bool? Active { get; set; }
    }
}