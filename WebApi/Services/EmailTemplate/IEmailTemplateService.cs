using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Dtos;
using WebApi.Data.Entities;

namespace WebApi.Services
{
    public partial interface IEmailTemplateService
    {
        Task Delete(EmailTemplate emailTemplate);

        Task<ICollection<EmailDto>> EmailDto();

        Task<IQueryable<EmailTemplate>> GetAllQ();

        Task<EmailTemplate> GetById(int id);

        Task<EmailTemplate> GetByName(string name);

        Task Insert(EmailTemplate emailTemplate);

        Task<ICollection<EmailTemplate>> SearchByName(string name);

        Task Update(EmailTemplate emailTemplate);

        Task<EmailEditDto> GetEditById(int id);
    }
}