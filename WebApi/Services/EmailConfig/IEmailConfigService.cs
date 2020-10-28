using System.Threading.Tasks;
using WebApi.Data.Entities;
using WebApi.Dtos;

namespace WebApi.Services
{
    public partial interface IEmailConfigService
    {
        Task<EmailConfigDto> GetConfiguration();

        Task<EmailConfig> GetById(int id);

        Task Update(EmailConfigItemDto model);

        Task<EmailAccount> GetDefaultSender();

        Task<EmailAccount> GetDefaultReceiver();
    }
}