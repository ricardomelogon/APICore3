using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.Dtos;
using WebApi.Data.Entities;

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